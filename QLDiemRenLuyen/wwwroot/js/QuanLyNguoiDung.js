// Animation for table rows
document.addEventListener('DOMContentLoaded', function () {
    const rows = document.querySelectorAll('.modern-table tbody tr');
    rows.forEach((row, index) => {
        row.style.animationDelay = `${index * 0.1}s`;
        row.classList.add('animate__animated', 'animate__fadeInUp');
    });
});

// Enhanced modal interactions
document.querySelectorAll('[data-bs-toggle="modal"]').forEach(button => {
    button.addEventListener('click', function () {
        const targetModal = document.querySelector(this.getAttribute('data-bs-target'));
        if (targetModal) {
            targetModal.classList.add('animate__animated', 'animate__zoomIn');
        }
    });
});

document.querySelectorAll('.password-toggle').forEach(toggle => {
    toggle.addEventListener('click', function () {
        const container = this.closest('.password-container');
        let input = container.querySelector('.password-input');
        let textElement = container.querySelector('.password-text');

        if (input) {
            // For input fields in modals
            if (input.type === 'password') {
                input.type = 'text';
                this.classList.remove('fa-eye-slash');
                this.classList.add('fa-eye');
            } else {
                input.type = 'password';
                this.classList.remove('fa-eye');
                this.classList.add('fa-eye-slash');
            }
        } else if (textElement) {
            // For password text in table
            if (this.classList.contains('fa-eye-slash')) {
                textElement.textContent = textElement.dataset.password;
                this.classList.remove('fa-eye-slash');
                this.classList.add('fa-eye');
            } else {
                textElement.textContent = '••••••••';
                this.classList.remove('fa-eye');
                this.classList.add('fa-eye-slash');
            }
        }
    });
});