var fsElem = document.documentElement;
function openFullscreen() {
  if (fsElem.requestFullscreen) {
    fsElem.requestFullscreen();
  } else if (fsElem.mozRequestFullScreen) { /* Firefox */
    fsElem.mozRequestFullScreen();
  } else if (fsElem.webkitRequestFullscreen) { /* Chrome, Safari & Opera */
    fsElem.webkitRequestFullscreen();
  } else if (fsElem.msRequestFullscreen) { /* IE/Edge */
    fsElem.msRequestFullscreen();
  }
}

function closeFullscreen() {
  if (document.exitFullscreen) {
    document.exitFullscreen();
  } else if (document.mozCancelFullScreen) {
    document.mozCancelFullScreen();
  } else if (document.webkitExitFullscreen) {
    document.webkitExitFullscreen();
  } else if (document.msExitFullscreen) {
    document.msExitFullscreen();
  }
}

function toggleFullscreen() {
    if (
        document.fullscreenElement ||
        document.mozFullscreenElement ||
        document.webkitFullscreenElement ||
        document.msFullscreenElement
    )
    {
        closeFullscreen();
    }
    else
    {
        openFullscreen();
    }
}