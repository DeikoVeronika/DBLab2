document.getElementById('dateInput').onchange = function () {
    if (this.valueAsDate < new Date(2023, 1, 1)) {
        document.getElementById('dateError').textContent = 'Дата не може бути меншою за 1 січня 2023';
    } else {
        document.getElementById('dateError').textContent = '';
    }
};