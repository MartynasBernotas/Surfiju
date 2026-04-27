window.stripeInterop = {
    init: function (publishableKey) {
        window._stripe = Stripe(publishableKey);
        const elements = window._stripe.elements();
        window._cardElement = elements.create('card');
        window._cardElement.mount('#card-element');
    },
    confirmPayment: async function (clientSecret) {
        const result = await window._stripe.confirmCardPayment(clientSecret, {
            payment_method: { card: window._cardElement }
        });
        if (result.error) return { success: false, error: result.error.message };
        return { success: true };
    }
};
