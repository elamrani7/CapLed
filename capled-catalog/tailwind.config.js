/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: "class",
  theme: {
    extend: {
      colors: {
        primary: "#0056A6",
        accent: "#00AEEF",
        warning: "#FFB200",
        danger: "#D92B2B",
      }
    },
  },
  plugins: [],
}
