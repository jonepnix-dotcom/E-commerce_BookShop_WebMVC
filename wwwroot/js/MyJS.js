
function adjustQuantity(change, id) {
    var input = document.getElementById(id);
    var qty = parseInt(input.value);
    if (!isNaN(qty)) {
        qty += change;
        if (qty < 1) qty = 1; // Minimum quantity
        if (qty > 300) qty = 300; // Maximum quantity
        input.value = qty;
    }
}

function validateInput(input) {
    // Allow only numbers, set limits
    if (input.value < 1) {
        input.value = 1;
    } else if (input.value > 300) {
        input.value = 300;
    }
}
$(function () {
    // Ban đầu ẩn khối chứa partial view
    $("#searchResults").hide();

    // Khi người dùng click vào link tìm kiếm (hoặc xử lý sự kiện sau khi submit form)
    $(".search-nav").on("click", function (e) {
        e.preventDefault();
        $("#searchResults").toggle(); // Thay đổi trạng thái ẩn/hiện
    });
    $('#close-search').on('click', function () {
        $('#searchResults').hide(); // Ẩn container chứa bảng
    });
});
$(function () {
    $("#searchForm").on("submit", function (e) {
        e.preventDefault(); // Ngăn submit mặc định (reload trang)

        var searchString = $("#search").val().trim();

        $.ajax({
            url: $(this).attr("action"),
            type: "POST",
            data: { searchstring: searchString },
            success: function (data) {
                $("#resultTable").html(data);
                // Nếu muốn hiển thị searchResults khi có dữ liệu:
                $("#searchResults").show();
            },
            error: function (err) {
                console.error("Lỗi khi tìm kiếm:", err);
            }
        });
    });
});
