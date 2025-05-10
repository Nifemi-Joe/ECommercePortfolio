// wwwroot/js/site.js
document.addEventListener('DOMContentLoaded', function() {
    // Initialize sidebar behavior
    initSidebar();

    // Initialize animations
    initAnimations();

    // Initialize Bootstrap tooltips and popovers if available
    initBootstrapComponents();

    // Add smooth scrolling for anchor links
    initSmoothScroll();

    // Initialize any charts or graphs on the page
    initCharts();
});

/**
 * Initializes sidebar behavior including toggling, responsive design, and active links
 */
function initSidebar() {
    const body = document.body;
    const sidebarToggleBtn = document.getElementById('sidebarToggleBtn');
    const closeSidebarBtn = document.getElementById('closeSidebarBtn');
    const sidebar = document.getElementById('sidebar');

    // Create backdrop element for mobile
    const backdrop = document.createElement('div');
    backdrop.className = 'sidebar-backdrop';
    document.body.appendChild(backdrop);

    // Toggle sidebar on button click
    if (sidebarToggleBtn) {
        sidebarToggleBtn.addEventListener('click', function() {
            if (window.innerWidth < 992) {
                // On mobile, open/close sidebar
                body.classList.toggle('sidebar-open');
            } else {
                // On desktop, collapse/expand sidebar
                body.classList.toggle('sidebar-collapsed');

                // Save preference in localStorage
                const isCollapsed = body.classList.contains('sidebar-collapsed');
                localStorage.setItem('sidebarCollapsed', isCollapsed);
            }
        });
    }

    // Close sidebar on close button click (mobile)
    if (closeSidebarBtn) {
        closeSidebarBtn.addEventListener('click', function() {
            body.classList.remove('sidebar-open');
        });
    }

    // Close sidebar when backdrop is clicked (mobile)
    backdrop.addEventListener('click', function() {
        body.classList.remove('sidebar-open');
    });

    // Restore sidebar state from localStorage
    const savedCollapsed = localStorage.getItem('sidebarCollapsed');
    if (savedCollapsed === 'true' && window.innerWidth >= 992) {
        body.classList.add('sidebar-collapsed');
    }

    // Handle window resize
    window.addEventListener('resize', function() {
        if (window.innerWidth < 992) {
            body.classList.remove('sidebar-collapsed');
            body.classList.remove('sidebar-open');
        }
    });

    // Add click event listeners to sidebar navigation links
    const sidebarLinks = document.querySelectorAll('.sidebar-link');
    sidebarLinks.forEach(link => {
        link.addEventListener('click', function() {
            // Add loading indicator
            const loadingOverlay = document.createElement('div');
            loadingOverlay.className = 'loading-overlay';
            loadingOverlay.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';
            document.body.appendChild(loadingOverlay);

            // Close sidebar on mobile when link is clicked
            if (window.innerWidth < 992) {
                body.classList.remove('sidebar-open');
            }

            // Remove loading overlay if page doesn't change within 2 seconds
            // (this happens if the link was to the current page)
            setTimeout(() => {
                if (document.body.contains(loadingOverlay)) {
                    document.body.removeChild(loadingOverlay);
                }
            }, 2000);
        });
    });
}

/**
 * Initializes animations for content elements
 */
function initAnimations() {
    // Set up intersection observer for animation on scroll
    const observerOptions = {
        root: null,
        rootMargin: '0px',
        threshold: 0.1
    };

    const observer = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);

    // Observe elements with animation classes
    document.querySelectorAll('.feature-card, .stat-card, .section-title, .welcome-image').forEach(el => {
        observer.observe(el);
    });

    // Add hover animations to buttons
    document.querySelectorAll('.btn').forEach(btn => {
        btn.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px)';
            this.style.boxShadow = '0 4px 15px rgba(0, 0, 0, 0.15)';
        });

        btn.addEventListener('mouseleave', function() {
            this.style.transform = '';
            this.style.boxShadow = '';
        });
    });
}

/**
 * Initializes Bootstrap tooltips and popovers if available
 */
function initBootstrapComponents() {
    // Initialize tooltips if Bootstrap is available
    if (typeof bootstrap !== 'undefined') {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function(tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });

        // Initialize popovers
        const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        popoverTriggerList.map(function(popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });
    }
}

/**
 * Adds smooth scrolling behavior to anchor links
 */
function initSmoothScroll() {
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            const href = this.getAttribute('href');

            // Skip if href is just "#" or empty
            if (href === '#' || href === '') return;

            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                e.preventDefault();

                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
}

/**
 * Initializes charts and graphs if present on the page
 */
function initCharts() {
    // Initialize Chart.js charts if available
    if (typeof Chart !== 'undefined') {
        // Example chart initialization - customize as needed for your app
        const salesChartElement = document.getElementById('salesChart');
        if (salesChartElement) {
            const ctx = salesChartElement.getContext('2d');

            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                    datasets: [{
                        label: 'Sales',
                        data: [12, 19, 3, 5, 2, 3],
                        backgroundColor: 'rgba(26, 115, 232, 0.2)',
                        borderColor: 'rgba(26, 115, 232, 1)',
                        borderWidth: 2,
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    scales: {
                        y: {
                            beginAtZero: true
                        }
                    }
                }
            });
        }

        // Similar initialization for other charts
        const productChartElement = document.getElementById('productChart');
        if (productChartElement) {
            const ctx = productChartElement.getContext('2d');

            new Chart(ctx, {
                type: 'doughnut',
                data: {
                    labels: ['Electronics', 'Clothing', 'Food', 'Books', 'Other'],
                    datasets: [{
                        data: [35, 25, 20, 15, 5],
                        backgroundColor: [
                            'rgba(26, 115, 232, 0.7)',
                            'rgba(23, 78, 166, 0.7)',
                            'rgba(251, 188, 4, 0.7)',
                            'rgba(15, 157, 88, 0.7)',
                            'rgba(95, 99, 104, 0.7)'
                        ],
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'bottom'
                        }
                    }
                }
            });
        }
    }
}

/**
 * Adds a loading indicator to page transitions
 */
function showLoading() {
    const loadingOverlay = document.createElement('div');
    loadingOverlay.className = 'loading-overlay';
    loadingOverlay.innerHTML = '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>';
    document.body.appendChild(loadingOverlay);
}

/**
 * Handles form submissions to show loading indicator
 */
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', function() {
            // Check if form is valid before showing loading
            if (this.checkValidity()) {
                showLoading();
            }
        });
    });

    // Add click handler to all buttons that look like submit buttons
    document.querySelectorAll('button[type="submit"], input[type="submit"]').forEach(button => {
        button.addEventListener('click', function() {
            const form = this.closest('form');
            if (form && form.checkValidity()) {
                showLoading();
            }
        });
    });
});