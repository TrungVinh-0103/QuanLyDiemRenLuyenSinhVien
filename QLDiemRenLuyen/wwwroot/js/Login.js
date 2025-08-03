// Toggle password visibility
function togglePassword(button) {
    const passwordInput = button.parentElement.querySelector('input[type="password"], input[type="text"]')
    const icon = button.querySelector("i")

    if (passwordInput.type === "password") {
        passwordInput.type = "text"
        icon.classList.remove("fa-eye")
        icon.classList.add("fa-eye-slash")
    } else {
        passwordInput.type = "password"
        icon.classList.remove("fa-eye-slash")
        icon.classList.add("fa-eye")
    }
}

// Form validation and animations
document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector("#loginForm")
    const inputs = document.querySelectorAll(".form-control-custom")
    const submitBtn = document.querySelector(".btn-login")

    // Input focus animations
    inputs.forEach((input) => {
        input.addEventListener("focus", function () {
            this.parentElement.style.transform = "scale(1.02)"
        })

        input.addEventListener("blur", function () {
            this.parentElement.style.transform = "scale(1)"
        })

        // Real-time validation
        input.addEventListener("input", function () {
            if (this.value.trim() !== "") {
                this.style.borderColor = "#28a745"
            } else {
                this.style.borderColor = "#e9ecef"
            }
        })
    })

    // Form submission with loading animation
    if (form) {
        form.addEventListener("submit", (e) => {
            const username = document.querySelector('input[name="username"]').value.trim()
            const password = document.querySelector('input[name="password"]').value.trim()
            const role = document.querySelector('select[name="vaiTro"]').value

            if (!username || !password || !role) {
                e.preventDefault()

                // Show custom error
                const existingAlert = document.querySelector(".alert-danger-custom")
                if (existingAlert) {
                    existingAlert.remove()
                }

                const errorDiv = document.createElement("div")
                errorDiv.className = "alert-custom alert-danger-custom"
                errorDiv.innerHTML = '<i class="fas fa-exclamation-triangle me-2"></i>Vui lòng điền đầy đủ thông tin!'

                form.appendChild(errorDiv)

                // Auto remove after 3 seconds
                setTimeout(() => {
                    errorDiv.style.opacity = "0"
                    errorDiv.style.transform = "translateY(-20px)"
                    setTimeout(() => errorDiv.remove(), 300)
                }, 3000)
            } else {
                // Show loading animation
                submitBtn.classList.add("loading")
                submitBtn.innerHTML = '<span style="opacity: 0;">Đang đăng nhập...</span>'
            }
        })
    }

    // Parallax effect for floating shapes
    document.addEventListener("mousemove", (e) => {
        const shapes = document.querySelectorAll(".shape")
        const x = e.clientX / window.innerWidth
        const y = e.clientY / window.innerHeight

        shapes.forEach((shape, index) => {
            const speed = (index + 1) * 0.5
            const xPos = (x - 0.5) * speed * 20
            const yPos = (y - 0.5) * speed * 20

            shape.style.transform += ` translate(${xPos}px, ${yPos}px)`
        })
    })

    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll(".alert-custom")
    alerts.forEach((alert) => {
        setTimeout(() => {
            alert.style.opacity = "0"
            alert.style.transform = "translateY(-20px)"
            setTimeout(() => {
                alert.remove()
            }, 300)
        }, 5000)
    })

    // Auto-focus first input
    const firstInput = document.querySelector('input[name="username"]')
    if (firstInput) {
        firstInput.focus()
    }
})

// Keyboard shortcuts
document.addEventListener("keydown", (e) => {
    // Enter key to submit form
    if (e.key === "Enter" && e.target.tagName !== "BUTTON") {
        const form = document.querySelector("#loginForm")
        if (form) {
            form.submit()
        }
    }
})

// Caps Lock detection
document.addEventListener("keydown", (e) => {
    if (e.getModifierState && e.getModifierState("CapsLock")) {
        const capsWarning = document.getElementById("capsWarning")
        if (!capsWarning) {
            const warning = document.createElement("div")
            warning.id = "capsWarning"
            warning.className = "alert-custom alert-info-custom"
            warning.innerHTML = '<i class="fas fa-keyboard me-2"></i>Caps Lock đang bật'
            warning.style.fontSize = "0.8rem"
            warning.style.padding = "8px 15px"
            warning.style.marginTop = "10px"

            const passwordField = document.querySelector('input[type="password"]').parentElement.parentElement
            if (passwordField) {
                passwordField.appendChild(warning)
            }
        }
    } else {
        const capsWarning = document.getElementById("capsWarning")
        if (capsWarning) {
            capsWarning.remove()
        }
    }
})

// Prevent multiple form submissions
let isSubmitting = false
document.addEventListener("submit", (e) => {
    if (isSubmitting) {
        e.preventDefault()
        return false
    }
    isSubmitting = true

    // Reset after 3 seconds (in case of error)
    setTimeout(() => {
        isSubmitting = false
    }, 3000)
})

// Add smooth scroll for any internal links
document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
    anchor.addEventListener("click", function (e) {
        e.preventDefault()
        const target = document.querySelector(this.getAttribute("href"))
        if (target) {
            target.scrollIntoView({
                behavior: "smooth",
                block: "start",
            })
        }
    })
})

// Add loading state management
function showLoading() {
    const submitBtn = document.querySelector(".btn-login")
    if (submitBtn) {
        submitBtn.classList.add("loading")
        submitBtn.disabled = true
    }
}

function hideLoading() {
    const submitBtn = document.querySelector(".btn-login")
    if (submitBtn) {
        submitBtn.classList.remove("loading")
        submitBtn.disabled = false
    }
}

// Export functions for global use
window.togglePassword = togglePassword
window.showLoading = showLoading
window.hideLoading = hideLoading
