const menuToggle = document.querySelector(".menu-toggle");
const navLinks = document.querySelector(".nav-links");

menuToggle.addEventListener("click", () => {
  navLinks.classList.toggle("open");

  // Toggle aria-expanded for accessibility
  const expanded = menuToggle.getAttribute("aria-expanded") === "true";
  menuToggle.setAttribute("aria-expanded", !expanded);
});

function bookNow() {
  alert(
    "Booking feature coming soon! Please contact us to book your appointment."
  );
}
