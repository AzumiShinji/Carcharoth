$(document).ready(function () {
    animateDiv($('.a'));
});

function makeNewPosition()
{
    var h = $(window).height() - 50;
    var w = $(window).width() - 50;

    var nh = Math.floor(Math.random() * h);
    var nw = Math.floor(Math.random() * w);

    return [nh, nw];
}

function animateDiv($target) {
    var newq = makeNewPosition($target.parent());
    var oldq = $target.offset();
    var speed = calcSpeed([oldq.top, oldq.left], newq);
    $target.each(function () {
        //var a = getRandomInt(-360, 360);
        //$(this).css({
        //    'transform': 'rotate(' + a + 'deg) scale(' + calcScale(0.1, 0.5) + ')',
        //});
        //$(this).css({
        //    'transform': 'rotate(' + a + 'deg)',
        //});
    });
    $target.animate({
        top: newq[0],
        left: newq[1]
    }, speed, function () {
            animateDiv($target);
    });
};

function calcSpeed(prev, next) {

    var x = Math.abs(prev[1] - next[1]);
    var y = Math.abs(prev[0] - next[0]);

    var greatest = x > y ? x : y;

    var speedModifier = Math.random(20.05, 20.01);

    var speed = Math.ceil(greatest*2 / speedModifier);

    return speed;

}
function calcScale(min, max) {
    return Math.random() < 0.5 ? ((1 - Math.random()) * (max - min) + min) : (Math.random() * (max - min) + min);
}
function getRandomInt(min, max) {
    min = Math.ceil(min);
    max = Math.floor(max);
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function addNew($target) {
    var count = document.getElementById("knt9").childElementCount;
    if (count > 60) { alert('!'); return; }
    for (var i = 0; i < 2; i++) {
        var tag = document.createElement("div");
        tag.style.top = ($target).style.top;
        tag.style.left = ($target).style.left;
        tag.onclick = (function () { addNew(this); });
        var name = 'knt9_' + getRandomInt(0, 999);
        tag.className = name;
        var element = document.getElementById("knt9");
        element.appendChild(tag);
        animateDiv($('.' + name));
    }
}