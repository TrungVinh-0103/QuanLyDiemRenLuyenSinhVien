document.addEventListener('DOMContentLoaded', function () {
    const sidebarToggle = document.getElementById('sidebarToggle');
    const adminSidebar = document.getElementById('adminSidebar');
    const adminMain = document.getElementById('adminMain');
    const loadingOverlay = document.getElementById('loadingOverlay');

    // Sidebar toggle functionality
    sidebarToggle.addEventListener('click', function () {
        adminSidebar.classList.toggle('collapsed');
        adminMain.classList.toggle('sidebar-collapsed');

        // Save state to localStorage
        const isCollapsed = adminSidebar.classList.contains('collapsed');
        localStorage.setItem('sidebarCollapsed', isCollapsed);
    });

    // Restore sidebar state from localStorage
    const savedState = localStorage.getItem('sidebarCollapsed');
    if (savedState === 'true') {
        adminSidebar.classList.add('collapsed');
        adminMain.classList.add('sidebar-collapsed');
    }

    // Mobile sidebar toggle
    if (window.innerWidth <= 1024) {
        sidebarToggle.addEventListener('click', function () {
            adminSidebar.classList.toggle('mobile-open');
        });

        // Close sidebar when clicking outside on mobile
        document.addEventListener('click', function (e) {
            if (window.innerWidth <= 1024 &&
                !adminSidebar.contains(e.target) &&
                !sidebarToggle.contains(e.target)) {
                adminSidebar.classList.remove('mobile-open');
            }
        });
    }

    // Active nav link highlighting
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.sidebar-nav-link');

    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });

    // Smooth page transitions
    const links = document.querySelectorAll('a[href^="/Admin/"]');
    links.forEach(link => {
        link.addEventListener('click', function (e) {
            if (this.getAttribute('href') !== currentPath) {
                loadingOverlay.classList.add('show');
            }
        });
    });

    // Hide loading overlay after page load
    window.addEventListener('load', function () {
        setTimeout(() => {
            loadingOverlay.classList.remove('show');
        }, 500);
    });

    // Parallax effect for floating elements
    document.addEventListener('mousemove', function (e) {
        const shapes = document.querySelectorAll('.floating-circle');
        const x = e.clientX / window.innerWidth;
        const y = e.clientY / window.innerHeight;

        shapes.forEach((shape, index) => {
            const speed = (index + 1) * 0.3;
            const xPos = (x - 0.5) * speed * 50;
            const yPos = (y - 0.5) * speed * 50;

            shape.style.transform += ` translate(${xPos}px, ${yPos}px)`;
        });
    });

    // Keyboard shortcuts
    document.addEventListener('keydown', function (e) {
        // Ctrl + B to toggle sidebar
        if (e.ctrlKey && e.key === 'b') {
            e.preventDefault();
            sidebarToggle.click();
        }
    });

    // Auto-hide loading on navigation
    window.addEventListener('beforeunload', function () {
        loadingOverlay.classList.add('show');
    });
});

// Utility functions
function showNotification(message, type = 'info') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} position-fixed`;
    notification.style.cssText = `
                top: 90px;
                right: 20px;
                z-index: 9999;
                min-width: 300px;
                animation: slideInRight 0.3s ease;
            `;
    notification.innerHTML = `
                <i class="fas fa-${type === 'success' ? 'check-circle' : type === 'error' ? 'exclamation-triangle' : 'info-circle'} me-2"></i>
                ${message}
                <button type="button" class="btn-close" onclick="this.parentElement.remove()"></button>
            `;

    document.body.appendChild(notification);

    // Auto remove after 5 seconds
    setTimeout(() => {
        if (notification.parentElement) {
            notification.style.animation = 'slideOutRight 0.3s ease';
            setTimeout(() => notification.remove(), 300);
        }
    }, 5000);
}

// Export for global use
window.showNotification = showNotification;