(function () {
    // ===== Score Gauge (SVG) + Decorative Dots =====
    var score = window.__reportData.healthScore;
    document.querySelectorAll('.score-dots-container').forEach(function (dotsContainer) {
        var rings = [
            { radius: 99,  count: 28, size: 4,  opacity: 1.0 },
            { radius: 112, count: 25, size: 5,  opacity: 0.7 },
            { radius: 130, count: 25, size: 7,  opacity: 0.3}
        ];
        rings.forEach(function (ring) {
            for (var i = 0; i < ring.count; i++) {
                var angle = (Math.PI * 2 / ring.count) * i;
                var x = ring.radius * Math.cos(angle);
                var y = ring.radius * Math.sin(angle);
                var dot = document.createElement('div');
                dot.className = 'score-dot';
                dot.style.width  = ring.size + 'px';
                dot.style.height = ring.size + 'px';
                dot.style.opacity = ring.opacity;
                dot.style.left = (x - ring.size / 2) + 'px';
                dot.style.top  = (y - ring.size / 2) + 'px';
                dotsContainer.appendChild(dot);
            }
        });
    });

    var circumference = 628;
    document.querySelectorAll('.score-ring').forEach(function (ring) {
        var offset = circumference - (score / 100) * circumference;
        ring.style.strokeDashoffset = offset;
    });
    document.querySelectorAll('.score-num').forEach(function (numEl) {
        numEl.textContent = score;
    });

    // ===== QR Code =====
    try {
        var qr = qrcode(0, 'M');
        qr.addData(window.__reportData.qrUrl);
        qr.make();
        var qrSvg = qr.createSvgTag(4, 0);
        document.querySelectorAll('.qr-code').forEach(function (el) {
            el.innerHTML = qrSvg;
        });
    } catch (e) { }

    window.__chartsReady = true;
})();
