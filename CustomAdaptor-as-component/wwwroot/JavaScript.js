window.selectContent = function (id) {
    document.getElementsByClassName("e-flm_optrdiv")[0].style.display = "none";
    var btns = document.querySelector('.e-dialog').querySelectorAll('.e-btn');
    for (var i = 0; i < btns.length; i++)
    {

        btns[i].style.display = "none";

    }

}
