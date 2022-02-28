

// Set what to store or print (0/1)
const WRITE_BLOCKS_SUMMARY = true;
const WRITE_BLOCKS_TO_DISK = true;
const PRINT_FINAL_HASH = true;
const PRINT_FINAL_HASH_IN_SUMMARY = true;

function start() {
  console.time("Block1");
  if (Block1() == -1) {
    printf("\nCollision not found!\n");
  }
  console.timeEnd("Block1");
  console.time("Block2");
  if (Block2() == -1) {
    printf("\nCollision not found!\n");
  }
  console.timeEnd("Block2");
  // printHx(obj.Hx);
  // console.log("Final", obj);
  //
  console.log(
    "Colliding hash 0: ",
    (A0>>>0).toString(16),
    (B0>>>0).toString(16),
    (C0>>>0).toString(16),
    (D0>>>0).toString(16)
  );
  console.log(
    "Colliding hash 1: ",
    (A1>>>0).toString(16),
    (B1>>>0).toString(16),
    (C1>>>0).toString(16),
    (D1>>>0).toString(16)
  );
  const a1 = document.createElement("a");
  const a2 = document.createElement("a");
  a1.innerHTML = "File 1";
  a2.innerHTML = "File 2";
  a1.download = "f1.txt";
  a2.download = "f2.txt";
  a1.href = window.URL.createObjectURL(new Blob([new Uint8Array(v1)]));
  a2.href = window.URL.createObjectURL(new Blob([new Uint8Array(v2)]));
  document.body.appendChild(a1);
  document.body.appendChild(a2);
}

function toHex(x) {
  let s = '';
  s += (x >>> 24 & 0xFF).toString(16).padStart(2, "0");
  s += (x >>> 16 & 0xFF).toString(16).padStart(2, "0");
  s += (x >>> 8 & 0xFF).toString(16).padStart(2, "0");
  s += (x >>> 0 & 0xFF).toString(16).padStart(2, "0");
  return s;
}

document.getElementById("start").addEventListener("click", function () {
  seed = parseInt(document.getElementById("seed").value, 16);
  X = seed >>> 0;
  console.log('seed', seed >>> 0);
  start();
})
