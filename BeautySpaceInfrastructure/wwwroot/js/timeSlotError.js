function validateInput(inputElementId, errorMessageId, minValue, maxValue) {
    var inputValue = document.getElementById(inputElementId).value;
    var errorMessage = document.getElementById(errorMessageId);

    if (inputValue < minValue || inputValue > maxValue) {
        errorMessage.style.display = "inline";
    } else {
        errorMessage.style.display = "none";
    }
}

document.getElementById("dateInput").addEventListener("input", function () {
    validateInput("dateInput", "dateError", "2023-01-01", Infinity); // Infinity використовується як maxValue, оскільки немає максимальної дати
});

document.getElementById("startTimeInput").addEventListener("input", function () {
    validateInput("startTimeInput", "startTimeError", "07:00", "21:45");
});

document.getElementById("endTimeInput").addEventListener("input", function () {
    validateInput("endTimeInput", "endTimeError", "07:15", "22:00");
});
