// Function to automatically add spacing to phone number form input
document.getElementById('phone').addEventListener("keyup", function(event){
    const key = event.key;
    let txt = this.value;

    if ((txt.length===4 || txt.length===8) && key !=="Backspace")
        this.value=this.value+" ";
});
// Function to automatically add spacing to TFN form input
document.getElementById('tfn').addEventListener("keyup", function(event){
    const key = event.key;
    let txt = this.value;
    if ((txt.length===3 || txt.length===7) && key !=="Backspace")
        this.value=this.value+" ";
});
