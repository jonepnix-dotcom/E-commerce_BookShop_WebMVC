let originalData = {};

function enterEditMode() {
    document.querySelector('.profile-card').classList.add('editing');
    document.querySelector('.edit-btn').style.display = 'none';
    document.querySelector('.save-btn').style.display = 'flex';
    document.querySelector('.cancel-btn').style.display = 'flex';
    document.querySelector('.changepass-btn').style.display = 'none';
    document.querySelector('.openmap').style.display = 'flex';

    originalData = {
        name: document.getElementById('nameDisplay').textContent,
        email: document.getElementById('emailDisplay').textContent,
        birthday: document.getElementById('birthdayDisplay').textContent,
        address: document.getElementById('addressDisplay').textContent,
        phone: document.getElementById('phoneDisplay').textContent
    };

    document.getElementById('name').value = originalData.name;
    document.getElementById('email').value = originalData.email;
    document.getElementById("birthday").value = new Date().toISOString().split('T')[0];
    document.getElementById('address').value = originalData.address;
    document.getElementById('phones').value = originalData.phone;
}

function saveChanges() {
    document.querySelector('.profile-card').classList.remove('editing');
    document.querySelector('.edit-btn').style.display = 'flex';
    document.querySelector('.save-btn').style.display = 'none';
    document.querySelector('.cancel-btn').style.display = 'none';
    document.querySelector('.changepass-btn').style.display = 'flex';
    document.querySelector('.openmap').style.display = 'none';

    document.getElementById('nameDisplay').textContent = document.getElementById('name').value;
    document.getElementById('emailDisplay').textContent = document.getElementById('email').value;
    const isoDate = document.getElementById('birthday').value;
    let displayDate = "";
    if (isoDate) {
        const parts = isoDate.split('-');
        if (parts.length === 3) {
            displayDate = `${parts[2]}/${parts[1]}/${parts[0]}`;
        }
    }
    document.getElementById('birthdayDisplay').textContent = displayDate;
    document.getElementById('addressDisplay').textContent = document.getElementById('address').value;
    document.getElementById('phoneDisplay').textContent = document.getElementById('phones').value;
}

function cancelEdit() {
    document.querySelector('.profile-card').classList.remove('editing');
    document.querySelector('.edit-btn').style.display = 'flex';
    document.querySelector('.save-btn').style.display = 'none';
    document.querySelector('.cancel-btn').style.display = 'none';
    document.querySelector('.changepass-btn').style.display = 'flex';
    document.querySelector('.openmap').style.display = 'none';

    document.getElementById('nameDisplay').textContent = originalData.name;
    document.getElementById('emailDisplay').textContent = originalData.email;
    document.getElementById('birthdayDisplay').textContent = originalData.birthday;
    document.getElementById('addressDisplay').textContent = originalData.address;
    document.getElementById('phoneDisplay').textContent = originalData.phone;

    var textDangerElements = document.getElementsByClassName('text-danger');

    for (let i = 0; i < textDangerElements.length; i++) {
        textDangerElements[i].textContent = '';
    }
}

// Khi người dùng bấm "Đồng ý" trong modal
function confirmSave() {
    // Gọi hàm saveChanges() thật
    saveChanges();
    closeModal();
}
function enterChangePassword() {
    $('#changePasswordModal').show();
}
function cancelChangePassword() {
    $('#changePasswordModal').hide();
}
function submitChangePassword(e) {
    // Lấy ô input
    const passVal = document.getElementById('Password').value.trim();
    const rePassVal = document.getElementById('RePassword').value.trim();

    // Lấy tất cả lỗi .text-danger trong modal
    const errorElems = document.querySelectorAll('#changePasswordModal .modal-content .text-danger');
    let hasError = false;
    for (let i = 0; i < errorElems.length; i++) {
        if (errorElems[i].textContent.trim().length > 0) {
            hasError = true;
            break;
        }
    }

    // Nếu 2 ô trống hoặc đang có lỗi => chặn submit
    if (passVal === "" || rePassVal === "" || hasError) {
        e.preventDefault(); // Ngăn submit
        return;
    }
    alert("Đổi mật khẩu thành công");
}
// Khi bấm "Hủy" trong modal
function closeModal() {
    $('#confirmModal').hide();
}
function closeErrorModal() {
    $('#errorModal').hide();
}
function checkValidationAndOpenConfirmModal() {
    
    // Kích hoạt jQuery validation
    if ($("form").valid()) {
        // form valid => tiếp tục kiểm tra nội dung lỗi trong các span.text-danger
        var textDangerElements = document.querySelectorAll('#errorModal .modal-content .text-danger');
        var hasError = false;

        for (let i = 0; i < textDangerElements.length; i++) {
            // Kiểm tra nội dung textContent
            if (textDangerElements[i].textContent.trim().length > 0) {
                hasError = true;
                break; // thoát vòng lặp sớm
            }
        }

        if (hasError) {
            // Có lỗi => mở errorModal
            $('#errorModal').show();
        } else {
            // Không lỗi => mở confirmModal
            $('#confirmModal').show();
        }
    } else {
        // Form chưa valid => mở errorModal
        $('#errorModal').show();
    }
}
function togglePasswordVisibility() {
    const passInput = document.getElementById('Password');
    const rePassInput = document.getElementById('RePassword');
    const showPassCheck = document.getElementById('showPasswordCheck');

    if (showPassCheck.checked) {
        // Nếu checkbox được chọn => hiển thị mật khẩu
        passInput.type = 'text';
        rePassInput.type = 'text';
    } else {
        // Nếu bỏ chọn => ẩn mật khẩu
        passInput.type = 'password';
        rePassInput.type = 'password';
    }
}