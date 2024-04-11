document.getElementById('dateInput').onchange = function () {
    if (this.valueAsDate < new Date(2023, 1, 1)) {
        document.getElementById('dateError').textContent = 'Дата не може бути меншою за 1 січня 2023';
    } else {
        document.getElementById('dateError').textContent = '';
    }
};

document.getElementById('startTimeInput').onchange = function () {
    var inputTime = this.value;
    var minTime = "07:00";
    var maxTime = "21:45";

    if (inputTime < minTime || inputTime > maxTime) {
        document.getElementById('startTimeError').textContent = 'Час повинен бути між 07:00 та 21:45';
    } else {
        document.getElementById('startTimeError').textContent = '';
    }
};

document.getElementById('endTimeInput').onchange = function () {
    var inputTime = this.value;
    var minTime = "07:15";
    var maxTime = "22:00";

    if (inputTime < minTime || inputTime > maxTime) {
        document.getElementById('endTimeError').textContent = 'Час повинен бути між 07:15 та 22:00';
    } else {
        document.getElementById('endTimeError').textContent = '';
    }
};

