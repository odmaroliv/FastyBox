function initTheme() {
    if (localStorage.theme === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        document.documentElement.classList.add('dark');
        return true;
    } else {
        document.documentElement.classList.remove('dark');
        return false;
    }
}

function isDarkMode() {
    return document.documentElement.classList.contains('dark');
}

function toggleTheme() {
    if (document.documentElement.classList.contains('dark')) {
        document.documentElement.classList.remove('dark');
        localStorage.theme = 'light';
    } else {
        document.documentElement.classList.add('dark');
        localStorage.theme = 'dark';
    }
}

// Initialize Stripe Payment
function initializeStripePayment(clientSecret, publicKey, elementId, returnUrl) {
    const stripe = Stripe(publicKey);
    const options = {
        clientSecret: clientSecret,
        appearance: {
            theme: 'stripe',
            variables: {
                colorPrimary: '#27a873',
                colorBackground: document.documentElement.classList.contains('dark') ? '#1f2937' : '#ffffff',
                colorText: document.documentElement.classList.contains('dark') ? '#f3f4f6' : '#1f2937',
                colorDanger: '#ef4444',
                fontFamily: 'Inter, ui-sans-serif, system-ui, -apple-system, sans-serif',
                borderRadius: '0.5rem',
                spacingUnit: '4px',
            }
        },
    };

    const elements = stripe.elements(options);
    const paymentElement = elements.create('payment');
    paymentElement.mount('#' + elementId);

    const form = document.getElementById('payment-form');
    if (!form) {
        const newForm = document.createElement('form');
        newForm.id = 'payment-form';
        newForm.className = 'mt-6';
        document.getElementById(elementId).appendChild(newForm);

        // Create submit button
        const submitButton = document.createElement('button');
        submitButton.type = 'submit';
        submitButton.className = 'mt-4 w-full btn btn-success';
        submitButton.innerText = 'Pay Now';
        newForm.appendChild(submitButton);

        newForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            submitButton.disabled = true;
            submitButton.innerHTML = `
                <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Processing...
            `;

            const { error } = await stripe.confirmPayment({
                elements,
                confirmParams: {
                    return_url: returnUrl,
                }
            });

            if (error) {
                const messageElement = document.createElement('div');
                messageElement.className = 'mt-4 p-3 bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-200 rounded-lg';
                messageElement.innerText = error.message;
                document.getElementById(elementId).appendChild(messageElement);

                submitButton.disabled = false;
                submitButton.innerHTML = 'Pay Now';
            }
        });
    }
}