// ECommercePortfolio.Web/wwwroot/js/dashboard.js
// Dashboard specific JavaScript

document.addEventListener('DOMContentLoaded', function () {
    // Draw sales chart if the element exists
    const salesChartElement = document.getElementById('salesChart');
    if (salesChartElement) {
        drawSalesChart(salesChartElement);
    }

    // Draw product categories chart if the element exists
    const categoriesChartElement = document.getElementById('categoriesChart');
    if (categoriesChartElement) {
        drawCategoriesChart(categoriesChartElement);
    }
});

// Draw sales chart using Chart.js
function drawSalesChart(chartElement) {
    const ctx = chartElement.getContext('2d');

    // Sample data - in a real app, this would come from the server
    const salesData = {
        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
        datasets: [{
            label: 'Monthly Sales',
            data: [12500, 19000, 15000, 22000, 18000, 25000],
            backgroundColor: 'rgba(52, 152, 219, 0.2)',
            borderColor: 'rgba(52, 152, 219, 1)',
            borderWidth: 2,
            tension: 0.4
        }]
    };

    new Chart(ctx, {
        type: 'line',
        data: salesData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        callback: function(value) {
                            return '$' + value.toLocaleString();
                        }
                    }
                }
            },
            plugins: {
                title: {
                    display: true,
                    text: 'Monthly Sales Performance'
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            return '$' + context.raw.toLocaleString();
                        }
                    }
                }
            }
        }
    });
}

// Draw product categories chart using Chart.js
function drawCategoriesChart(chartElement) {
    const ctx = chartElement.getContext('2d');

    // Sample data - in a real app, this would come from the server
    const categoriesData = {
        labels: ['Electronics', 'Clothing', 'Home & Kitchen', 'Books', 'Sports & Outdoors'],
        datasets: [{
            label: 'Products by Category',
            data: [24, 18, 12, 15, 9],
            backgroundColor: [
                'rgba(52, 152, 219, 0.7)',
                'rgba(46, 204, 113, 0.7)',
                'rgba(155, 89, 182, 0.7)',
                'rgba(241, 196, 15, 0.7)',
                'rgba(231, 76, 60, 0.7)'
            ],
            borderWidth: 1
        }]
    };

    new Chart(ctx, {
        type: 'doughnut',
        data: categoriesData,
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Products by Category'
                },
                legend: {
                    position: 'right'
                }
            }
        }
    });
}