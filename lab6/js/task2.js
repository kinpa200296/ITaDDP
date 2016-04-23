function task(){
    var floorsCnt = parseInt(prompt("Enter floors quantity: "));
    var entriesCnt = parseInt(prompt("Enter entries quantity: "));
    var flatsPerFloor = parseInt(prompt("Enter quantity of flats per floor: "));
    var flatNumber = parseInt(prompt("Enter flat number: "));
    try {
        var flatsPerEntry = floorsCnt*flatsPerFloor;
        var entryNumber = Math.floor(flatNumber/flatsPerEntry);
        if (flatNumber % flatsPerEntry != 0){
            entryNumber += 1;
        }
        if (entryNumber >= 1 && entryNumber <= entriesCnt){
            alert("You live in entry number " + entryNumber);
        }
        else{
            throw new Error("Bad input!");
        }
    }
    catch (ex){
        alert("An error encountered: " + ex);
    }
}
task();