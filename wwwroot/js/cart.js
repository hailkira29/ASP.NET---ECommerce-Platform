// Cart functionality for E-Commerce Platform
$(document).ready(function () {
    // Load cart count on page load
    updateCartCount();

    // Handle Add to Cart buttons
    $('.add-to-cart-btn').on('click', function (e) {
        e.preventDefault();
        
        const productId = $(this).data('product-id');
        const quantity = $(this).closest('.product-card').find('.quantity-input').val() || 1;
        
        addToCart(productId, quantity);
    });

    // Handle quantity updates in cart
    $('.quantity-input').on('change', function () {
        const productId = $(this).data('product-id');
        const quantity = $(this).val();
        
        if (quantity > 0) {
            updateCartQuantity(productId, quantity);
        }
    });

    // Handle remove item buttons
    $('.remove-item-btn').on('click', function (e) {
        e.preventDefault();
        
        const productId = $(this).data('product-id');
        removeFromCart(productId);
    });

    // Handle clear cart button
    $('#clear-cart-btn').on('click', function (e) {
        e.preventDefault();
        
        if (confirm('Are you sure you want to clear your cart?')) {
            clearCart();
        }
    });
});

function addToCart(productId, quantity) {
    $.ajax({
        url: '/Cart/AddToCart',
        type: 'POST',
        data: {
            productId: productId,
            quantity: quantity,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (response) {
            if (response.success) {
                updateCartCount(response.cartCount);
                showToast('Product added to cart!', 'success');
            } else {
                showToast('Error adding product to cart', 'error');
            }
        },
        error: function () {
            showToast('Error adding product to cart', 'error');
        }
    });
}

function updateCartQuantity(productId, quantity) {
    $.ajax({
        url: '/Cart/UpdateQuantity',
        type: 'POST',
        data: {
            productId: productId,
            quantity: quantity,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (response) {
            if (response.success) {
                // Update item total price
                $(`[data-product-id="${productId}"]`).closest('.cart-item').find('.item-total').text(response.totalPrice);
                
                // Update cart total
                $('.cart-total').text(response.cartTotal);
                
                // Update cart count
                updateCartCount(response.cartCount);
                
                showToast('Cart updated!', 'success');
            } else {
                showToast('Error updating cart', 'error');
            }
        },
        error: function () {
            showToast('Error updating cart', 'error');
        }
    });
}

function removeFromCart(productId) {
    $.ajax({
        url: '/Cart/RemoveItem',
        type: 'POST',
        data: {
            productId: productId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        },
        success: function (response) {
            if (response.success) {
                // Remove the item row
                $(`[data-product-id="${productId}"]`).closest('.cart-item').fadeOut(300, function() {
                    $(this).remove();
                    
                    // Check if cart is empty
                    if ($('.cart-item').length === 0) {
                        location.reload();
                    }
                });
                
                // Update cart total
                $('.cart-total').text(response.cartTotal);
                
                // Update cart count
                updateCartCount(response.cartCount);
                
                showToast('Item removed from cart!', 'success');
            } else {
                showToast('Error removing item from cart', 'error');
            }
        },
        error: function () {
            showToast('Error removing item from cart', 'error');
        }
    });
}

function clearCart() {
    $.ajax({
        url: '/Cart/Clear',
        type: 'POST',
        data: {
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function () {
            location.reload();
        },
        error: function () {
            showToast('Error clearing cart', 'error');
        }
    });
}

function updateCartCount(count) {
    if (count !== undefined) {
        $('#cart-count').text(count);
        
        if (count > 0) {
            $('#cart-count').removeClass('d-none').addClass('d-inline');
        } else {
            $('#cart-count').addClass('d-none').removeClass('d-inline');
        }
    } else {
        // Fetch cart count from server
        $.ajax({
            url: '/Cart/GetCartCount',
            type: 'GET',
            success: function (response) {
                updateCartCount(response.cartCount);
            },
            error: function () {
                console.log('Error fetching cart count');
            }
        });
    }
}

function showToast(message, type) {
    // Create toast element
    const toastId = 'toast-' + Date.now();
    const toastClass = type === 'success' ? 'bg-success' : 'bg-danger';
    
    const toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-white ${toastClass} border-0" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    
    // Add toast container if it doesn't exist
    if ($('#toast-container').length === 0) {
        $('body').append('<div id="toast-container" class="toast-container position-fixed top-0 end-0 p-3"></div>');
    }
    
    // Add toast to container
    $('#toast-container').append(toastHtml);
    
    // Show toast
    const toast = new bootstrap.Toast(document.getElementById(toastId));
    toast.show();
    
    // Remove toast element after it's hidden
    document.getElementById(toastId).addEventListener('hidden.bs.toast', function () {
        $(this).remove();
    });
}
