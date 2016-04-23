function task(){

    function fibonachi(x){
        if (x < 2){
            return x;
        }
        else{
            return fibonachi(x-1)+fibonachi(x-2);
        }
    }

    var number = parseInt(prompt("Enter what number of the fibonachi sequence you want to know: "));

    try {
        if (number < 0 || isNaN(number)) {
            throw new Error("number cannot be negative!");
        }
        else{
            alert(fibonachi(number));
        }
    }
    catch (ex) {
        alert("An error encountered: " + ex);
    }
}
task();
