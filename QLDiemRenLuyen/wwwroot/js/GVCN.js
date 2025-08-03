// Navbar scroll effect
window.addEventListener('scroll', function () {
    const navbar = document.getElementById('mainNavbar');
    if (window.scrollY > 50) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }
});

// Active nav link highlighting
document.addEventListener('DOMContentLoaded', function () {
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.nav-link-custom');

    navLinks.forEach(link => {
        link.classList.remove('active');
        const href = link.getAttribute('href');

        // Check for asp-controller and asp-action attributes
        const controller = link.getAttribute('asp-controller');
        const action = link.getAttribute('asp-action');

        if (href === currentPath ||
            (controller && action && currentPath.includes(`/${controller}/${action}`))) {
            link.classList.add('active');
        }
    });
});

// Parallax effect for floating elements
window.addEventListener('scroll', function () {
    const scrolled = window.scrollY;
    const parallax = document.querySelectorAll('.floating-circle');
    const speed = 0.5;

    parallax.forEach((element, index) => {
        const yPos = -(scrolled * speed * (index + 1) * 0.3);
        element.style.transform = `translateY(${yPos}px) rotate(${scrolled * 0.1}deg)`;
    });
});

// Add ripple effect to buttons
function createRipple(event) {
    const button = event.currentTarget;
    const circle = document.createElement("span");
    const diameter = Math.max(button.clientWidth, button.clientHeight);
    const radius = diameter / 2;

    circle.style.width = circle.style.height = `${diameter}px`;
    circle.style.left = `${event.clientX - button.offsetLeft - radius}px`;
    circle.style.top = `${event.clientY - button.offsetTop - radius}px`;
    circle.classList.add("ripple");

    const ripple = button.getElementsByClassName("ripple")[0];
    if (ripple) {
        ripple.remove();
    }

    button.appendChild(circle);
}

// Apply ripple effect to nav links
document.querySelectorAll('.nav-link-custom').forEach(button => {
    button.addEventListener('click', createRipple);
});

// Add loading animation
window.addEventListener('beforeunload', function () {
    document.body.style.opacity = '0.7';
    document.body.style.transition = 'opacity 0.3s ease';
});

// Table row hover animation
document.querySelectorAll('.table tbody tr').forEach(row => {
    row.addEventListener('mouseenter', function () {
        this.style.transform = 'scale(1.01)';
        this.style.transition = 'all 0.3s ease';
    });

    row.addEventListener('mouseleave', function () {
        this.style.transform = 'scale(1)';
    });
});