// Set what to store or print (0/1)
const WRITE_BLOCKS_SUMMARY = true;
const WRITE_BLOCKS_TO_DISK = true;
const PRINT_FINAL_HASH = true;
const PRINT_FINAL_HASH_IN_SUMMARY = true;

function createFileDownload(content, text, filename) {
  const a1 = document.createElement("a");
  a1.innerHTML = text;
  a1.download = filename;
  a1.target ="_blank"
  a1.href = window.URL.createObjectURL(new Blob([new Uint8Array(content)]));
  return a1;
}

function setSelectedBlock(block) {
  const activeBlock = block == 1? "block1" : "block2";
  const inactiveBlock = block == 1? "block2" : "block1";
  document.getElementById(inactiveBlock).classList.remove("active");
  document.getElementById(activeBlock).classList.add("active");
}

function toHex(x) {
  let s = "";
  s += ((x >>> 0) & 0xff).toString(16).padStart(2, "0");
  s += ((x >>> 8) & 0xff).toString(16).padStart(2, "0");
  s += ((x >>> 16) & 0xff).toString(16).padStart(2, "0");
  s += ((x >>> 24) & 0xff).toString(16).padStart(2, "0");
  return s;
}

function newCollision(a, b, contentA, contentB, hash) {
  const item = document.createElement('li');
  const div = document.createElement('div');
  div.classList.add('block');
  const divA = document.createElement('div');
  const divB = document.createElement('div');
  divA.innerText = a;
  divB.innerText = b;
  div.appendChild(divA);
  div.appendChild(divB);
  const a1 = createFileDownload(contentA, "M0", `m0-${hash}.txt`)
  const a2 = createFileDownload(contentB, "M1", `m1-${hash}.txt`)
  const section = document.createElement("section");
  const heading = document.createElement("h4");
  heading.innerText = hash;
  item.appendChild(section);
  section.appendChild(heading);
  section.appendChild(a1);
  section.appendChild(a2);
  item.appendChild(div);
  document.getElementById('blocks').appendChild(item)
}

function updateStats(stats) {
  for(let key of Object.keys(stats)) {
    const id = `stat-${key}`;
    const element = document.getElementById(id);
    const value = stats[key];
    if(element && element.innerText !== value) {
      element.innerText = value;
    }
  }
}

document.getElementById("showcontent").addEventListener('change', function (e) {
  if(e.target.checked) {
    document.getElementById("blocks").classList.add("show")
  } else {
    document.getElementById("blocks").classList.remove("show")
  }
});
