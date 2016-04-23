function task() {
    while (true) {
        var firstNum = prompt("Enter first number: ");
        firstNum = parseFloat(firstNum);
        if (isNaN(firstNum)) {
            alert("First input is not a number");
            break;
        }
        var secondNum = prompt("Enter second number: ");
        secondNum = parseFloat(secondNum);
        if (isNaN(secondNum)) {
            alert("Second input is not a number");
            break;
        }
        if (firstNum === secondNum) {
            alert("Numbers are equal");
        }
        else if (firstNum < secondNum) {
            alert("First number is lower");
        }
        else if (firstNum > secondNum) {
            alert("Second number is lower");
        }
    }
}
task();