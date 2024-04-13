function validateInput(inputElementId, errorMessageId, minValue, maxValue) {
    var inputValue = document.getElementById(inputElementId).value;
    var errorMessage = document.getElementById(errorMessageId);

    if (inputValue < minValue || inputValue > maxValue) {
        errorMessage.style.display = "inline";
    } else {
        errorMessage.style.display = "none";
    }
}

function validateTimeDifference(startTimeId, endTimeId, minDifference, errorMessageId) {
    var startTime = document.getElementById(startTimeId).value;
    var endTime = document.getElementById(endTimeId).value;
    var errorMessage = document.getElementById(errorMessageId);

    var difference = new Date(`1970-01-01T${endTime}:00`) - new Date(`1970-01-01T${startTime}:00`);
    var differenceInMinutes = difference / 1000 / 60;

    if (differenceInMinutes < minDifference) {
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
    validateTimeDifference("startTimeInput", "endTimeInput", 15, "minServiceEndTimeError");
});

document.getElementById("endTimeInput").addEventListener("input", function () {
    validateInput("endTimeInput", "endTimeError", "07:15", "22:00");
    validateTimeDifference("startTimeInput", "endTimeInput", 15, "minServiceEndTimeError");
});
