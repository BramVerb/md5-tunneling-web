// Set what to store or print (0/1)
const WRITE_BLOCKS_SUMMARY = true;
const WRITE_BLOCKS_TO_DISK = true;
const PRINT_FINAL_HASH = true;
const PRINT_FINAL_HASH_IN_SUMMARY = true;

function createFileDownload(content, text, filename) {
  const a1 = document.createElement("a");
  a1.innerHTML = text;
  // a1.download = filename;
  a1.target ="_blank"
  a1.href = window.URL.createObjectURL(new Blob([new Uint8Array(content)]));
  return a1;
}

function step1() {
  console.time("Block1");
  if (Block1() == -1) {
    printf("\nCollision not found!\n");
    return -1;
  }
  console.timeEnd("Block1");
  document.getElementById("block1").classList.remove("active");
  document.getElementById("block1").innerHTML += " - DONE";
  document.getElementById("block2").classList.add("active");
  setTimeout(step2, 100);
}

function step2() {
  console.time("Block2");
  if (Block2() == -1) {
    printf("\nCollision not found!\n");
    return -1;
  }
  console.timeEnd("Block2");
  setTimeout(stepHash, 100);
}

function stepHash() {
  document.getElementById("block2").classList.remove("active");
  document.getElementById("block2").innerHTML += " - DONE";
  document.getElementById("hash").classList.add("active");
  let obj = createMD5Object();
  obj.Hx[0] = 0x00000080;
  obj.Hx[14] = 0x00000400;
  obj.a = A0;
  obj.b = B0;
  obj.c = C0;
  obj.d = D0;
  obj = HMD5Tr(obj);
  A0 += obj.a;
  B0 += obj.b;
  C0 += obj.c;
  D0 += obj.d;
  const hash = toHex(A0)+toHex(B0)+toHex(C0)+toHex(D0);
  console.log("Collision hash:", hash);
  document.getElementById("hash-content").innerHTML = hash;
  setTimeout(end, 100);
}

function end() {
  document.getElementById("hash").classList.remove("active");
  document.getElementById("hash").innerHTML += " - DONE";
  document.getElementById("files").classList.add("active");
  const a1 = createFileDownload(v1, "File 1", "file1.txt");
  const a2 = createFileDownload(v2, "File 2", "file1.txt");
  document.getElementById("files-content").appendChild(a1);
  document.getElementById("files-content").appendChild(a2);
  document.getElementById("start").removeAttribute("disabled");
  document.getElementById("files").innerHTML += " - DONE";
}

function start() {
  document.getElementById("inputs").classList.remove("active");
  document.getElementById("block1").classList.add("active");
  document.getElementById("start").setAttribute('disabled', true);
  setTimeout(step1, 100);



    // Last message block computation (Padding)


  // Hash computation
}

function toHex(x) {
  let s = "";
  s += ((x >>> 0) & 0xff).toString(16).padStart(2, "0");
  s += ((x >>> 8) & 0xff).toString(16).padStart(2, "0");
  s += ((x >>> 16) & 0xff).toString(16).padStart(2, "0");
  s += ((x >>> 24) & 0xff).toString(16).padStart(2, "0");
  return s;
}

document.getElementById("start").addEventListener("click", function () {
  seed = parseInt(document.getElementById("seed").value, 16);
  X = seed >>> 0;
  console.log("seed", seed >>> 0);
  console.time("full");
  start();
  console.timeEnd("full");
});
X = 3770369038;

function newBlock1(a, b) {
  const item = document.createElement('li');
  const div = document.createElement('div');
  div.classList.add('block');
  const divA = document.createElement('div');
  const divB = document.createElement('div');
  divA.innerText = a;
  divB.innerText = b;
  div.appendChild(divA);
  div.appendChild(divB);
  item.appendChild(div);
  document.getElementById('blocks').appendChild(item)
}
document.getElementById("gpu").addEventListener("click", function () {
  document.getElementById("gpu").setAttribute("disabled", true);
  gpu();
})
