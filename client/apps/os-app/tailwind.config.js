/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      animation: {
        slightYMove: 'slightYMove 10s ease-in-out infinite',
      },
      keyframes: {
        slightYMove: {
          '0%, 100%': { transform: 'translateY(0)' },
          '50%': { transform: 'translateY(4px)' },
        },
      },
      colors: {
        "primary": "#E06365",
        "background": '#101010',
        "secondary": "#777777"
      }
    },
    // font sizes
    fontSize: {
      "3xl": "48px",
      "2xl": "38px",
      xl: "32px",
      lg: "24px",
      md: "18px",
      sm: "15px",
      xs: "12px",
    }
  },
  plugins: [],
}

