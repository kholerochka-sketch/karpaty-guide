// ================== НАЛАШТУВАННЯ ==================
// Адреса вашого API. Порт дивіться в консолі після "dotnet run"
// (рядок "Now listening on: http://localhost:5xxx"). Закінчується на /api, без слеша в кінці.
const API_URL = "http://localhost:5000/api";

// true  = показувати тестові дані (корисно, поки бекенд ще не готовий)
// false = брати дані з реального API
const USE_DEMO_DATA = true;
// ===================================================

const CATEGORY_LABELS = { waterfall: "Водоспад", viewpoint: "Оглядовий майданчик", meadow: "Полонина" };
const CATEGORY_ICONS = { waterfall: "💧", viewpoint: "🔭", meadow: "🌿" };
const DIFFICULTY_LABELS = { easy: "легкий маршрут", medium: "середній маршрут", hard: "складний маршрут" };

// Тестові дані (вигадані, лише для верстки)
const DEMO_PLACES = [
    {
        id: "1", name: "Тестовий водоспад", category: "waterfall", region: "Карпати", difficulty: "easy",
        description: "Невеликий водоспад у лісі. Це тестовий запис, поки API не підключене.", latitude: 48.5, longitude: 24.0
    },
    {
        id: "2", name: "Тестовий оглядовий майданчик", category: "viewpoint", region: "Карпати", difficulty: "medium",
        description: "Місце з видом на хребти. Тестовий запис для перевірки верстки.", latitude: 48.6, longitude: 24.2
    },
    {
        id: "3", name: "Тестова полонина", category: "meadow", region: "Карпати", difficulty: "hard",
        description: "Широка гірська лука. Тестовий запис для перевірки верстки.", latitude: 48.4, longitude: 24.4
    },
];
const DEMO_TIPS = [
    { id: "1", title: "Тестова порада", text: "Візьміть воду, теплий одяг і зарядіть телефон.", category: "gear" },
];

// ---------- Допоміжні функції ----------
const $ = (selector) => document.querySelector(selector);

function escapeHtml(value) {
    const div = document.createElement("div");
    div.textContent = value ?? "";
    return div.innerHTML;
}

function setStatus(element, message, type = "") {
    element.textContent = message;
    element.className = "status" + (type ? " " + type : "");
}

async function apiGet(path) {
    const response = await fetch(API_URL + path);
    if (!response.ok) throw new Error("Помилка сервера: " + response.status);
    return response.json();
}

// ---------- Місця ----------
let allPlaces = [];

async function loadPlaces(category = "") {
    const status = $("#placesStatus");
    setStatus(status, "Завантаження...");
    try {
        if (USE_DEMO_DATA) {
            allPlaces = category ? DEMO_PLACES.filter((p) => p.category === category) : DEMO_PLACES;
        } else {
            const query = category ? "?category=" + encodeURIComponent(category) : "";
            allPlaces = await apiGet("/places" + query);
        }
        renderPlaces(allPlaces);
        setStatus(status, allPlaces.length ? "" : "У цій категорії поки немає місць.");
    } catch (error) {
        console.error(error);
        $("#placesGrid").innerHTML = "";
        setStatus(status, "Не вдалося завантажити місця. Перевірте, чи запущено API.", "error");
    }
}

function renderPlaces(places) {
    const grid = $("#placesGrid");
    grid.innerHTML = places.map((place) => `
    <article class="card" data-id="${escapeHtml(place.id)}" tabindex="0">
      <div class="card__top card__top--${escapeHtml(place.category)}">${CATEGORY_ICONS[place.category] || "⛰️"}</div>
      <div class="card__body">
        <h3>${escapeHtml(place.name)}</h3>
        <p class="card__meta">${escapeHtml(place.region)} · ${escapeHtml(DIFFICULTY_LABELS[place.difficulty] || place.difficulty)}</p>
        <p class="card__text">${escapeHtml(shorten(place.description, 110))}</p>
      </div>
    </article>
  `).join("");
}

function shorten(text, max) {
    if (!text) return "";
    return text.length > max ? text.slice(0, max).trim() + "…" : text;
}

function openPlace(id) {
    const place = allPlaces.find((p) => String(p.id) === String(id));
    if (!place) return;
    const mapLink = `https://www.google.com/maps?q=${encodeURIComponent(place.latitude)},${encodeURIComponent(place.longitude)}`;
    $("#dialogBody").innerHTML = `
    <h3>${escapeHtml(place.name)}</h3>
    <p><span class="badge">${escapeHtml(CATEGORY_LABELS[place.category] || place.category)}</span>
       <span class="badge">${escapeHtml(DIFFICULTY_LABELS[place.difficulty] || place.difficulty)}</span></p>
    <p><strong>Регіон:</strong> ${escapeHtml(place.region)}</p>
    <p>${escapeHtml(place.description)}</p>
    <p><a href="${mapLink}" target="_blank" rel="noopener">Відкрити на карті</a></p>
  `;
    $("#placeDialog").showModal();
}

// ---------- Поради ----------
async function loadTips() {
    const status = $("#tipsStatus");
    setStatus(status, "Завантаження...");
    try {
        const tips = USE_DEMO_DATA ? DEMO_TIPS : await apiGet("/tips");
        $("#tipsList").innerHTML = tips.map((tip) => `
      <article class="tip">
        <h3>${escapeHtml(tip.title)}</h3>
        <p>${escapeHtml(tip.text)}</p>
      </article>
    `).join("");
        setStatus(status, tips.length ? "" : "Поради скоро з’являться.");
    } catch (error) {
        console.error(error);
        setStatus(status, "Не вдалося завантажити поради.", "error");
    }
}

// ---------- Форма відгуку ----------
async function submitFeedback(event) {
    event.preventDefault();
    const status = $("#fbStatus");
    const button = $("#fbSubmit");

    const data = {
        name: $("#fbName").value.trim(),
        email: $("#fbEmail").value.trim(),
        rating: Number($("#fbRating").value),
        message: $("#fbMessage").value.trim(),
    };

    // Проста перевірка на боці сайту (сервер перевіряє ще раз)
    if (data.name.length < 2) return setStatus(status, "Введіть ім’я (мінімум 2 символи).", "error");
    if (!/^\S+@\S+\.\S+$/.test(data.email)) return setStatus(status, "Введіть коректний email.", "error");
    if (data.message.length < 10) return setStatus(status, "Повідомлення має містити мінімум 10 символів.", "error");

    button.disabled = true;
    setStatus(status, "Надсилання...");

    try {
        if (USE_DEMO_DATA) {
            await new Promise((resolve) => setTimeout(resolve, 600)); // імітація
        } else {
            const response = await fetch(API_URL + "/feedback", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data),
            });
            if (!response.ok) {
                let message = "Не вдалося надіслати відгук.";
                try {
                    const problem = await response.json();
                    const firstError = problem.errors && Object.values(problem.errors)[0];
                    if (firstError && firstError[0]) message = firstError[0];
                } catch { /* відповідь без JSON, лишаємо загальне повідомлення */ }
                throw new Error(message);
            }
        }
        setStatus(status, "Дякуємо! Ваш відгук надіслано.", "ok");
        $("#feedbackForm").reset();
    } catch (error) {
        console.error(error);
        setStatus(status, error.message || "Помилка з’єднання з сервером.", "error");
    } finally {
        button.disabled = false;
    }
}

// ---------- Події ----------
document.addEventListener("DOMContentLoaded", () => {
    loadPlaces();
    loadTips();

    $("#filters").addEventListener("click", (event) => {
        const button = event.target.closest(".filter");
        if (!button) return;
        document.querySelectorAll(".filter").forEach((b) => b.classList.remove("active"));
        button.classList.add("active");
        loadPlaces(button.dataset.category);
    });

    $("#placesGrid").addEventListener("click", (event) => {
        const card = event.target.closest(".card");
        if (card) openPlace(card.dataset.id);
    });
    $("#placesGrid").addEventListener("keydown", (event) => {
        if (event.key !== "Enter") return;
        const card = event.target.closest(".card");
        if (card) openPlace(card.dataset.id);
    });

    $("#dialogClose").addEventListener("click", () => $("#placeDialog").close());
    $("#feedbackForm").addEventListener("submit", submitFeedback);
});