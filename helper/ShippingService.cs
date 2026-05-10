using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;

namespace TheLight_JoneBookShop_WebMVC.helper
{
    public class ShippingService
    {
        private readonly HttpClient _httpClient;
        private const string BASE_URL = "https://online-gateway.ghn.vn/shiip/public-api";
        private const string TOKEN = "e879a4aa-fb5c-11ef-b76a-6ad60f756aea";  // Thay bằng token của bạn
        private const int SHOP_ID = 5673334;
        private const int FIXED_FROM_DISTRICT_ID = 1536; // Trảng Dài, Biên Hoà, Đồng Nai

        public ShippingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            if (!_httpClient.DefaultRequestHeaders.Contains("Token"))
            {
                _httpClient.DefaultRequestHeaders.Add("Token", TOKEN);
            }
        }

        // Hàm loại bỏ dấu tiếng Việt
        string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();
            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        // Lấy danh sách tỉnh
        public async Task<JObject> GetProvincesAsync()
        {
            var response = await _httpClient.GetAsync($"{BASE_URL}/master-data/province");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }

        // Lấy danh sách huyện theo ProvinceID
        public async Task<JObject> GetDistrictsAsync(int provinceId)
        {
            var response = await _httpClient.GetAsync($"{BASE_URL}/master-data/district?province_id={provinceId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }

        // Lấy danh sách phường/xã theo DistrictID
        public async Task<JObject> GetWardsAsync(int districtId)
        {
            var response = await _httpClient.GetAsync($"{BASE_URL}/master-data/ward?district_id={districtId}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }

        // Lấy dịch vụ vận chuyển khả dụng dựa vào District của nơi nhận
        public async Task<int> GetAvailableServiceAsync(int recipientDistrictId)
        {
            var url = $"{BASE_URL}/v2/shipping-order/available-services?shop_id={SHOP_ID}&from_district={FIXED_FROM_DISTRICT_ID}&to_district={recipientDistrictId}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            // Giả sử service_id được trả về trong data[0]["service_id"]
            var service = json["data"]?.FirstOrDefault();
            if (service == null)
                return 0;
            return service["service_id"]!.Value<int>();
        }

        // Tính phí vận chuyển qua API fee
        public async Task<JObject> GetShippingFeeAsync(int serviceId, int recipientDistrictId, string wardCode, int insurance_value, int height, int length, int weight, int width)
        {
            var url = $"{BASE_URL}/v2/shipping-order/fee?service_id={serviceId}" +
                      $"&insurance_value={insurance_value}&from_district_id={FIXED_FROM_DISTRICT_ID}" +
                      $"&to_district_id={recipientDistrictId}&to_ward_code={wardCode}" +
                      $"&height={height}&length={length}&weight={weight}&width={width}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }

        // Phương thức tính phí vận chuyển dựa vào địa chỉ người nhận (không phân biệt dấu)
        // Định dạng địa chỉ: "Phường, Quận, Tỉnh, (Country)"
        public async Task<decimal> CalculateShippingFeeAsync(string recipientAddress, int insurance_value, int height, int length, int weight, int width)
        {
            // Chuẩn hóa địa chỉ bằng cách loại bỏ dấu tiếng Việt
            string normalizedAddress = RemoveDiacritics(recipientAddress);

            // --- Xử lý Tỉnh/Thành phố ---
            var provincesJson = await GetProvincesAsync();
            var matchedProvince = provincesJson["data"]?
                .FirstOrDefault(p =>
                {
                    var nameExtensions = p["NameExtension"]?.Values<string>();
                    if (nameExtensions != null)
                    {
                        return nameExtensions.Any(name =>
                            normalizedAddress.IndexOf(RemoveDiacritics(name!), StringComparison.OrdinalIgnoreCase) >= 0);
                    }
                    return false;
                });
            if (matchedProvince == null)
                throw new Exception("Không tìm thấy Tỉnh/Thành phố trong địa chỉ.");

            var provinceNameExtensions = matchedProvince["NameExtension"]?.Values<string>().ToList();
            if (provinceNameExtensions == null || !provinceNameExtensions.Any())
                throw new Exception("Không có tên mở rộng trong thông tin Tỉnh/Thành phố.");

            var matchingProvinceNames = provinceNameExtensions
                .Where(name => normalizedAddress.IndexOf(RemoveDiacritics(name!), StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
            if (!matchingProvinceNames.Any())
                throw new Exception("Không tìm thấy tên mở rộng phù hợp trong địa chỉ.");

            string provinceNameFound = matchingProvinceNames.OrderByDescending(name => name!.Length).First()!;
            int provinceId = matchedProvince["ProvinceID"]!.Value<int>();

            // Lấy phần của địa chỉ trước khi xuất hiện tên tỉnh đã tìm được
            int provinceIndex = normalizedAddress.IndexOf(RemoveDiacritics(provinceNameFound!), StringComparison.OrdinalIgnoreCase);
            if (provinceIndex <= 0)
                throw new Exception("Địa chỉ không hợp lệ, không tìm thấy thông tin trước Tỉnh.");
            string leftPart = recipientAddress.Substring(0, provinceIndex);

            // Tách phần bên trái theo dấu phẩy để lấy thông tin Quận và Phường
            var leftParts = leftPart.Split(',')
                                    .Select(x => x.Trim())
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .ToList();
            if (leftParts.Count < 2)
                throw new Exception("Địa chỉ không hợp lệ, không đủ thông tin Quận/Huyện và Phường/Xã.");

            // Giả sử phần cuối cùng là Quận/Huyện, phần kế cuối là Phường/Xã
            string districtNamePart = leftParts[leftParts.Count - 1];
            string wardPart = leftParts[leftParts.Count - 2];
            string wardNamePart = Regex.Replace(wardPart, @"^\d+\s*", "");

            // --- Xử lý Quận/Huyện ---
            var districtsJson = await GetDistrictsAsync(provinceId);
            var districtsList = districtsJson["data"]?.Values<JToken>().ToList();
            if (districtsList == null || !districtsList.Any())
                throw new Exception("Không có dữ liệu Quận/Huyện.");

            var matchingDistricts = districtsList.Where(d =>
            {
                if (d!["NameExtension"] != null)
                {
                    var districtExtList = d["NameExtension"]!.Values<string>().ToList();
                    return districtExtList.Any(name =>
                        normalizedAddress.IndexOf(RemoveDiacritics(name!), StringComparison.OrdinalIgnoreCase) >= 0);
                }
                else
                {
                    string candidate = d["DistrictName"]?.ToString() ?? "";
                    return normalizedAddress.IndexOf(RemoveDiacritics(candidate), StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }).ToList();
            if (!matchingDistricts.Any())
                throw new Exception("Không tìm thấy Quận/Huyện trong địa chỉ.");

            var matchedDistrict = matchingDistricts.OrderByDescending(d =>
            {
                if (d!["NameExtension"] != null)
                {
                    var ext = d["NameExtension"]!.Values<string>().OrderByDescending(n => n!.Length).First();
                    return ext!.Length;
                }
                else
                {
                    string candidate = d["DistrictName"]?.ToString() ?? "";
                    return candidate.Length;
                }
            }).First();

            string districtNameFound;
            if (matchedDistrict!["NameExtension"] != null)
            {
                var districtExtList = matchedDistrict["NameExtension"]!.Values<string>().ToList();
                var matchingDistrictNames = districtExtList
                    .Where(name => normalizedAddress.IndexOf(RemoveDiacritics(name!), StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                districtNameFound = matchingDistrictNames.Any()
                    ? matchingDistrictNames.OrderByDescending(name => name!.Length).First()
                    : matchedDistrict["DistrictName"]?.ToString();
            }
            else
            {
                districtNameFound = matchedDistrict["DistrictName"]?.ToString()!;
            }
            int recipientDistrictId = matchedDistrict["DistrictID"]!.Value<int>();

            // --- Xử lý Phường/Xã ---
            var wardsJson = await GetWardsAsync(recipientDistrictId);
            var wardsList = wardsJson["data"]?.Values<JToken>().ToList();
            if (wardsList == null || !wardsList.Any())
                throw new Exception("Không có dữ liệu Phường/Xã.");

            var matchingWards = wardsList.Where(w =>
            {
                if (w!["NameExtension"] != null)
                {
                    var wardExtList = w["NameExtension"]!.Values<string>().ToList();
                    return wardExtList.Any(name =>
                        normalizedAddress.IndexOf(RemoveDiacritics(name!), StringComparison.OrdinalIgnoreCase) >= 0);
                }
                else
                {
                    string candidate = w["WardName"]?.ToString() ?? "";
                    candidate = Regex.Replace(candidate, @"^\d+\s*", "");
                    return normalizedAddress.IndexOf(RemoveDiacritics(candidate), StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }).ToList();
            if (!matchingWards.Any())
                throw new Exception("Không tìm thấy Phường/Xã trong địa chỉ.");

            var matchedWard = matchingWards.OrderByDescending(w =>
            {
                if (w!["NameExtension"] != null)
                {
                    var wardExtList = w["NameExtension"]!.Values<string>().OrderByDescending(n => n!.Length).First();
                    return wardExtList!.Length;
                }
                else
                {
                    string candidate = w["WardName"]?.ToString() ?? "";
                    candidate = Regex.Replace(candidate, @"^\d+\s*", "");
                    return candidate.Length;
                }
            }).First();

            string wardNameFound;
            if (matchedWard!["NameExtension"] != null)
            {
                var wardExtList = matchedWard["NameExtension"]!.Values<string>().ToList();
                var matchingWardNames = wardExtList
                    .Where(name => normalizedAddress.IndexOf(RemoveDiacritics(name!), StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                wardNameFound = matchingWardNames.Any()
                    ? matchingWardNames.OrderByDescending(name => name!.Length).First()
                    : matchedWard["WardName"]?.ToString();
            }
            else
            {
                wardNameFound = matchedWard["WardName"]?.ToString()!;
            }
            string wardCode = matchedWard["WardCode"]!.ToString();

            // --- Lấy dịch vụ vận chuyển và tính phí ---
            int serviceId = await GetAvailableServiceAsync(recipientDistrictId);
            if (serviceId == 0)
                throw new Exception("Không tìm thấy dịch vụ vận chuyển phù hợp");
            var feeJson = await GetShippingFeeAsync(serviceId, recipientDistrictId, wardCode, insurance_value, height, length, weight, width);
            decimal fee = feeJson["data"]?["total"]?.Value<decimal>() ?? 0;
            return fee;
        }
    }
}
