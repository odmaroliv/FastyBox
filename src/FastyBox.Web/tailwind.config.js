// tailwind.config.js
module.exports = {
    content: [
        "./**/*.{razor,html,cshtml}"
    ],
    darkMode: 'class',
    theme: {
        extend: {
            colors: {
                primary: {
                    50: '#eefbf4',
                    100: '#d6f5e3',
                    200: '#b0eacc',
                    300: '#7ddab0',
                    400: '#4ac28c',
                    500: '#27a873',
                    600: '#19885c',
                    700: '#166b4b',
                    800: '#15553d',
                    900: '#144734',
                    950: '#0a2a1f',
                },
                secondary: {
                    50: '#eef8ff',
                    100: '#daefff',
                    200: '#bce3ff',
                    300: '#8ed0ff',
                    400: '#59b6ff',
                    500: '#3498fe',
                    600: '#1879f5',
                    700: '#1062e3',
                    800: '#1352b8',
                    900: '#154691',
                    950: '#112c57',
                },
                accent: '#FFC107',
                success: '#10B981',
                warning: '#F59E0B',
                danger: '#EF4444',
                info: '#3B82F6',
            },
            fontFamily: {
                sans: ['Inter', 'ui-sans-serif', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'Segoe UI', 'Roboto', 'Helvetica Neue', 'Arial', 'sans-serif'],
            },
            boxShadow: {
                'soft-sm': '0 2px 4px 0 rgba(0, 0, 0, 0.05)',
                'soft': '0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.03)',
                'soft-md': '0 6px 10px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.03)',
                'soft-lg': '0 10px 15px -3px rgba(0, 0, 0, 0.05), 0 4px 6px -2px rgba(0, 0, 0, 0.03)',
                'soft-xl': '0 20px 25px -5px rgba(0, 0, 0, 0.05), 0 10px 10px -5px rgba(0, 0, 0, 0.02)',
                'soft-2xl': '0 25px 50px -12px rgba(0, 0, 0, 0.04)',
            },
            borderRadius: {
                'xl': '1rem',
                '2xl': '1.5rem',
                '3xl': '2rem',
            },
        },
    },
    plugins: [
        require('@tailwindcss/forms'),
    ],
}
