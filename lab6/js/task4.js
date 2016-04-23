function task(){
    var monthLength = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    var weekdayNames = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    var month = parseInt(prompt("Enter month: "));
    var day = parseInt(prompt("Enter day: "));

    try {
        if (month < 1 || month > monthLength.length || monthLength[month - 1] < day || day < 1) {
            throw new Error("Bad input!");
        }
        else{
            var overallDays = 0;
            for (var i = 0; i < month - 1; i++){
                overallDays += monthLength[i];
            }
            overallDays += day;
            var dayNumber = (overallDays + 2)%7;
            if (!isNaN(dayNumber)) {
                alert(weekdayNames[dayNumber]);
            }
            else {
                throw new Error("Bad input!");
            }
        }
    }
    catch (ex) {
            alert("An error encountered: " + ex);
    }

}
task();