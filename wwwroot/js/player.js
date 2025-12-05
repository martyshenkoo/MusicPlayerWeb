(function () {
  const audio = document.getElementById("player");
  if (!audio) return;

  const srcEl = document.getElementById("playerSource");
  const now = document.getElementById("nowPlaying");
  const nowTitle = document.getElementById("nowPlayingTitle");
  const progress = document.getElementById("playerProgress");

  const btnPlay = document.getElementById("btnPlay");
  const btnPause = document.getElementById("btnPause");
  const btnStop = document.getElementById("btnStop");
  const btnBack = document.getElementById("btnBack");
  const btnFwd = document.getElementById("btnFwd");
  const btnShuffle = document.getElementById("btnShuffle");
  const btnRepeat = document.getElementById("btnRepeat");

  const rows = document.querySelectorAll("table.tracks tbody tr");
  const currentTrack = { title: "", url: "" };
  let skipPauseNotification = false;

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
    currentTrack.title = title ?? "";
    currentTrack.url = url ?? "";
    srcEl.src = url;
    audio.load();
    audio.play();
    const label = title ? `Зараз грає: ${title}` : "";
    now.textContent = label;
    if (nowTitle) nowTitle.textContent = title ?? "";
    setNowPlayingVisible(true);
  }

  function notifyCommand(action, payload) {
    const body = payload ? JSON.stringify(payload) : "{}";
    fetch(`/Player/${action}`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body
    }).catch(() => {});
  }

  btnPlay?.addEventListener("click", () => audio.play());
  btnPause?.addEventListener("click", () => audio.pause());
  btnStop?.addEventListener("click", () => {
    skipPauseNotification = true;
    audio.pause();
    audio.currentTime = 0;
    notifyCommand("Stop");
    setNowPlayingVisible(false);
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

  audio.addEventListener("play", () => {
    if (currentTrack.url) {
      notifyCommand("Play", {
        title: currentTrack.title,
        url: currentTrack.url
      });
      if (nowTitle) nowTitle.textContent = currentTrack.title || "";
    }
  });

  audio.addEventListener("pause", () => {
    if (skipPauseNotification) {
      skipPauseNotification = false;
      return;
    }
    if (!audio.ended) {
      notifyCommand("Pause");
    }
  });

  audio.addEventListener("ended", () => {
    skipPauseNotification = true;
    if (repeat) {
      audio.currentTime = 0;
      audio.play();
      return;
    }

    notifyCommand("Stop");
    setNowPlayingVisible(false);
  });

  // Прогрес-бар
  if (progress) {
    progress.addEventListener("input", (e) => {
      if (!audio.duration) return;
      const value = Number(e.target.value);
      audio.currentTime = (value / 100) * audio.duration;
    });

    audio.addEventListener("timeupdate", () => {
      if (!audio.duration) return;
      progress.value = ((audio.currentTime / audio.duration) * 100).toString();
    });

    audio.addEventListener("ended", () => {
      progress.value = "0";
    });

    audio.addEventListener("loadedmetadata", () => {
      progress.value = "0";
    });
  }

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
    currentTrack.title = first.getAttribute("data-title") ?? "";
    currentTrack.url = first.getAttribute("data-url") ?? "";
    srcEl.src = currentTrack.url;
    const label = currentTrack.title
      ? `Обрано: ${currentTrack.title}`
      : "";
    now.textContent = label;
    if (nowTitle) nowTitle.textContent = currentTrack.title || "";
  }

  function setNowPlayingVisible(visible) {
    const bar = document.querySelector(".now-playing-bar");
    if (!bar) return;

    if (visible) {
      bar.classList.add("visible");
    } else {
      bar.classList.remove("visible");
      now.textContent = "";
      if (nowTitle) nowTitle.textContent = "";
      if (progress) progress.value = "0";
    }
  }
})();
