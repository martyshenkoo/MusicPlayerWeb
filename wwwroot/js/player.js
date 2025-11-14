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
  const btnShuffle = document.getElementById("btnShuffle");
  const btnRepeat = document.getElementById("btnRepeat");

  const rows = document.querySelectorAll("table.tracks tbody tr");

  rows.forEach((r) => {
    const btn = r.querySelector(".playThis");
    if (!btn) return;
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

  btnPlay?.addEventListener("click", () => audio.play());
  btnPause?.addEventListener("click", () => audio.pause());
  btnStop?.addEventListener("click", () => {
    audio.pause();
    audio.currentTime = 0;
  });

  btnBack?.addEventListener("click", () => {
    audio.currentTime = Math.max(0, audio.currentTime - 10);
  });

  btnFwd?.addEventListener("click", () => {
    audio.currentTime = Math.min(audio.duration || 0, audio.currentTime + 10);
  });

  // Shuffle
  btnShuffle?.addEventListener("click", () => {
    const arr = Array.from(rows);
    if (arr.length === 0) return;
    const r = arr[Math.floor(Math.random() * arr.length)];
    playUrl(r.getAttribute("data-url"), r.getAttribute("data-title"));
  });

  // Repeat
  let repeat = false;
  btnRepeat?.addEventListener("click", () => {
    repeat = !repeat;
    btnRepeat.style.background = repeat ? "#e0ffe0" : "";
  });

  audio.addEventListener("ended", () => {
    if (repeat) {
      audio.currentTime = 0;
      audio.play();
    }
  });

  // Еквалайзер через Web Audio API
  try {
    const ctx = new AudioContext();
    const source = ctx.createMediaElementSource(audio);

    const bass = ctx.createBiquadFilter();
    bass.type = "lowshelf";
    bass.frequency.value = 200;

    const treble = ctx.createBiquadFilter();
    treble.type = "highshelf";
    treble.frequency.value = 2000;

    source.connect(bass).connect(treble).connect(ctx.destination);

    const bassSlider = document.getElementById("eqBass");
    const trebleSlider = document.getElementById("eqTreble");

    if (bassSlider) {
      bassSlider.oninput = (e) => {
        bass.gain.value = e.target.value;
      };
    }

    if (trebleSlider) {
      trebleSlider.oninput = (e) => {
        treble.gain.value = e.target.value;
      };
    }
  } catch {
    // Якщо Web Audio API не підтримується — просто ігноруємо еквалайзер
  }

  // Автовибір першого треку
  if (rows.length > 0) {
    const first = rows[0];
    srcEl.src = first.getAttribute("data-url");
    now.textContent = `Обрано: ${first.getAttribute("data-title")}`;
  }
})();
