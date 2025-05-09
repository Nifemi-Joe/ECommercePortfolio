// ECommercePortfolio.Web/wwwroot/js/product-management.js
// Product management specific JavaScript

document.addEventListener('DOMContentLoaded', function () {
    // Image preview functionality for product form
    const imageUrlInput = document.getElementById('ImageUrl');
    const imagePreview = document.getElementById('imagePreview');

    if (imageUrlInput && imagePreview) {
        // Initial preview
        updateImagePreview();

        // Update preview when URL changes
        imageUrlInput.addEventListener('change', updateImagePreview);
        imageUrlInput.addEventListener('keyup', updateImagePreview);
    }

    // Handle category filter
    const categoryFilter = document.getElementById('categoryFilter');
    if (categoryFilter) {
        categoryFilter.addEventListener('change', function() {
            window.location.href = `/Products/Index?categoryId=${this.value}`;
        });
    }

    // Initialize datatable if present
    const productTable = document.getElementById('productTable');
    if (productTable && typeof $.fn.DataTable !== 'undefined') {
        $('#productTable').DataTable({
            responsive: true,
            language: {
                search: "Search products:",
                lengthMenu: "Show _MENU_ products per page",
                info: "Showing _START_ to _END_ of _TOTAL_ products"
            },
            columnDefs: [
                { orderable: false, targets: -1 } // Disable sorting on action column
            ]
        });
    }
});

// Update image preview based on URL input
function updateImagePreview() {
    const imageUrlInput = document.getElementById('ImageUrl');
    const imagePreview = document.getElementById('imagePreview');

    if (imageUrlInput.value) {
        imagePreview.src = imageUrlInput.value;
        imagePreview.classList.remove('d-none');
    } else {
        imagePreview.classList.add('d-none');
    }
}

// Validate product form
function validateProductForm() {
    const name = document.getElementById('Name').value;
    const price = document.getElementById('Price').value;
    const stockQuantity = document.getElementById('StockQuantity').value;
    const category = document.getElementById('CategoryId').value;

    let isValid = true;
    let errorMessage = '';

    if (!name) {
        errorMessage += 'Product name is required.\n';
        isValid = false;
    }

    if (!price || isNaN(price) || parseFloat(price) <= 0) {
        errorMessage += 'Price must be a positive number.\n';
        isValid = false;
    }

    if (!stockQuantity || isNaN(stockQuantity) || parseInt(stockQuantity) < 0) {
        errorMessage += 'Stock quantity must be a non-negative number.\n';
        isValid = false;
    }

    if (!category) {
        errorMessage += 'Please select a category.\n';
        isValid = false;
    }

    if (!isValid) {
        alert(errorMessage);
    }

    return isValid;
}