(function () {
  const audio = document.getElementById("player");
  if (!audio) return;

  const srcEl = document.getElementById("playerSource");
  const now = document.getElementById("nowPlaying");

  const btnPlay = document.getElementById("btnPlay");
  const btnPause = document.getElementById("btnPause");
  const btnStop = document.getElementById("btnStop");
  const btnBack = document.getElementById("btnBack");
  const btnFwd = document.getElementById("btnFwd");

  const rows = document.querySelectorAll("table.tracks tbody tr");
  rows.forEach((r) => {
    const btn = r.querySelector(".playThis");
    btn.addEventListener("click", () => {
      const url = r.getAttribute("data-url");
      const title = r.getAttribute("data-title");
      playUrl(url, title);
    });
  });

  function playUrl(url, title) {
    if (!url) return;
    srcEl.src = url;
    audio.load();
    audio.play();
    now.textContent = `Зараз грає: ${title}`;
  }

  btnPlay.addEventListener("click", () => audio.play());
  btnPause.addEventListener("click", () => audio.pause());
  btnStop.addEventListener("click", () => {
    audio.pause();
    audio.currentTime = 0;
  });
  btnBack.addEventListener("click", () => {
    audio.currentTime = Math.max(0, audio.currentTime - 10);
  });
  btnFwd.addEventListener("click", () => {
    audio.currentTime = Math.min(audio.duration || 0, audio.currentTime + 10);
  });

  if (rows.length > 0) {
    const first = rows[0];
    srcEl.src = first.getAttribute("data-url");
    now.textContent = `Обрано: ${first.getAttribute("data-title")}`;
  }
})();
