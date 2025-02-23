const USE_B1_Q4 = true;
const USE_B1_Q9 = true;
const USE_B1_Q10 = true;
const USE_B1_Q13 = true;
const USE_B1_Q14 = true;
const USE_B1_Q20 = true;

const v1 = newArray(128);
const v2 = newArray(128);

// Mask generation for tunnel Q4 - 1 bit
const Q4_mask_bits = [26];
const Q4_strength = 1;
const mask_Q4 = generate_mask(Q4_strength, Q4_mask_bits);

// Mask generation for tunnel Q9 - 3 bits
const Q9_mask_bits = [22, 23, 24];
const Q9_strength = 3;
const mask_Q9 = generate_mask(Q9_strength, Q9_mask_bits);

// Mask generation for tunnel Q13 - 12 bits
const Q13_mask_bits = [2, 3, 5, 7, 10, 11, 12, 21, 22, 23, 28, 29];
const Q13_strength = 12;
const mask_Q13 = generate_mask(Q13_strength, Q13_mask_bits);

// Mask generation for tunnel Q20 - 6 bits
const Q20_mask_bits = [1, 2, 10, 15, 22, 24];
const Q20_strength = 6;
const mask_Q20 = generate_mask(Q20_strength, Q20_mask_bits);

// Mask generation for tunnel Q10 - 3 bits
const Q10_mask_bits = [11, 25, 27];
const Q10_strength = 3;
const mask_Q10 = generate_mask(Q10_strength, Q10_mask_bits);

// Mask generation for tunnel Q14 - 9 bits
const Q14_mask_bits = [1, 2, 3, 5, 6, 7, 27, 28, 29];
const Q14_strength = 9;
const mask_Q14 = generate_mask(Q14_strength, Q14_mask_bits);

class Block1CandidatesGenerator {
  constructor(seed, rngMask) {
    this.X = seed;
    this.Q = newArray(65);
    this.x = newArray(16);
    this.rngMask = rngMask;
  }

  rng() {
    this.X = (1664525 * this.X + 1013904223) & 0xffffffff;
    //X = (((((1103515245 >>> 0) * X >>>0) + 12345)>>>0) & 0xffffffff) >>> 0;

    return this.X & this.rngMask;
  }

  getnext(max_iterations) {
    const IV1 = 0x67452301;
    const IV2 = 0xefcdab89;
    const IV3 = 0x98badcfe;
    const IV4 = 0x10325476;
    let rng = this.rng.bind(this);
    let x = this.x;
    let Q = this.Q;
    const QM3 = IV1;
    const QM0 = IV2;
    const QM1 = IV3;
    const QM2 = IV4;
    if (!max_iterations) {
      max_iterations = 1;
    }
    let sigma_Q19 = 0;
    let sigma_Q20 = 0;
    let sigma_Q23 = 0;

    let lastSeed = this.X;
    let nowSeed = this.X;
    for (let it = 0; it < max_iterations; it++) {
      const seedNow = this.X >>> 0;
      lastSeed = nowSeed;
      nowSeed = seedNow;
      // Q[1]  = .... .... .... .... .... .... .... ....
      // RNG   = **** **** **** **** **** **** **** ****  0xffffffff
      // 0     = .... .... .... .... .... .... .... ....  0x00000000
      // 1     = .... .... .... .... .... .... .... ....  0x00000000
      Q[1] = rng();

      // Q[2] will be generated from x[1] using Q[14..17]

      // Q[3]  = .... .... .vvv 0vvv vvvv 0vvv v0.. ....
      // RNG   = **** **** **** .*** **** .*** *.** ****  0xfff7f7bf
      // 0     = .... .... .... *... .... *... .*.. ....  0x00080840
      // 1     = .... .... .... .... .... .... .... ....  0x00000000
      Q[3] = rng() & 0xfff7f7bf;

      // Q[4]  = 1... .... 0^^^ 1^^^ ^^^^ 1^^^ ^011 ....
      // RNG   = .*** **** .... .... .... .... .... ****  0x7f00000f
      // 0     = .... .... *... .... .... .... .*.. ....  0x00800040
      // 1     = *... .... .... *... .... *... ..** ....  0x80080830
      // Q[3]  = .... .... .*** .*** **** .*** *... ....  0x0077f780
      Q[4] = (rng() & 0x7f00000f) + 0x80080830 + (Q[3] & 0x0077f780);
      // Q[4] = (rng() & 0x7f00000f) >>> 0;

      // I set bit 2 and 4 to zero, not necessary for Q14 tunnel
      // Q[5]  = 1000 100v 0100 0000 0000 0000 0010 0101
      // RNG   = .... ...* .... .... .... .... .... ....  0x01000000
      // 0     = .*** .**. *.** **** **** **** **.* *.*.  0x76bfffda
      // 1     = *... *... .*.. .... .... .... ..*. .*.*  0x88400025
      Q[5] = (rng() & 0x01000000) + 0x88400025;

      // I set bit 2 and 4 to zero, not necessary for Q14 tunnel
      // Q[6]  = 0000 001^ 0111 1111 1011 1100 0100 0001
      // RNG   = .... .... .... .... .... .... .... ....  0x00000000
      // 0     = **** **.. *... .... .*.. ..** *.** ***.  0xfc8043be
      // 1     = .... ..*. .*** **** *.** **.. .*.. ...*  0x027fbc41
      // Q[5]  = .... ...* .... .... .... .... .... ....  0x01000000
      Q[6] = 0x027fbc41 + (Q[5] & 0x01000000);

      // Q[7]  = 0000 0011 1111 1110 1111 1000 0010 0000
      // RNG   = .... .... .... .... .... .... .... ....  0x00000000
      // 0     = **** **.. .... ...* .... .*** **.* ****  0xfc0107df
      // 1     = .... ..** **** ***. **** *... ..*. ....  0x03fef820
      Q[7] = 0x03fef820;

      // Q[8]  = 0000 0001 1..1 0001 0.0v 0101 0100 0000
      // RNG   = .... .... .**. .... .*.* .... .... ....  0x00605000
      // 0     = **** ***. .... ***. *.*. *.*. *.** ****  0xfe0eaabf
      // 1     = .... ...* *..* ...* .... .*.* .*.. ....  0x01910540
      Q[8] = (rng() & 0x00605000) + 0x01910540;

      // Q[9]  = 1111 1011 ...1 0000 0.1^ 1111 0011 1101
      // RNG   = .... .... ***. .... .*.. .... .... ....  0x00e04000
      // 0     = .... .*.. .... **** *... .... **.. ..*.  0x040f80c2
      // 1     = **** *.** ...* .... ..*. **** ..** **.*  0xfb102f3d
      // Q[8]  = .... .... .... .... ...* .... .... ....  0x00001000
      Q[9] = (rng() & 0x00e04000) + 0xfb102f3d + (Q[8] & 0x00001000);

      // Q[10] = 0111 .... 0001 1111 1v01 ...0 01.. ..00
      // RNG   = .... **** .... .... .*.. ***. ..** **..  0x0f004e3c
      // 0     = *... .... ***. .... ..*. ...* *... ..**  0x80e02183
      // 1     = .*** .... ...* **** *..* .... .*.. ....  0x701f9040
      Q[10] = (rng() & 0x0f004e3c) + 0x701f9040;

      // Q[11] = 0010 .0v0 111. 0001 1^00 .0.0 11.. ..10
      // RNG   = .... *.*. ...* .... .... *.*. ..** **..  0x0a100a3c
      // 0     = **.* .*.* .... ***. ..** .*.* .... ...*  0xd50e3501
      // 1     = ..*. .... ***. ...* *... .... **.. ..*.  0x20e180c2
      // Q[10] = .... .... .... .... .*.. .... .... ....  0x00004000
      Q[11] = (rng() & 0x0a100a3c) + 0x20e180c2 + (Q[10] & 0x00004000);

      // Q[12] = 000. ..^^ .... 1000 0001 ...1 0... ....
      // RNG   = ...* **.. **** .... .... ***. .*** ****  0x1cf00e7f
      // 0     = ***. .... .... .*** ***. .... *... ....  0xe007e080
      // 1     = .... .... .... *... ...* ...* .... ....  0x00081100
      // Q[11] = .... ..** .... .... .... .... .... ....  0x03000000
      Q[12] = (rng() & 0x1cf00e7f) + 0x00081100 + (Q[11] & 0x03000000);

      // Q[13] = 01.. ..01 .... 1111 111. ...0 0... 1...
      // RNG   = ..** **.. **** .... ...* ***. .*** .***  0x3cf01e77
      // 0     = *... ..*. .... .... .... ...* *... ....  0x82000180
      // 1     = .*.. ...* .... **** ***. .... .... *...  0x410fe008
      Q[13] = (rng() & 0x3cf01e77) + 0x410fe008;

      // Q[14] = 000. ..00 .... 1011 111. ...1 1... 1...
      // RNG   = ...* **.. **** .... ...* ***. .*** .***  0x1cf01e77
      // 0     = ***. ..** .... .*.. .... .... .... ....  0xe3040000
      // 1     = .... .... .... *.** ***. ...* *... *...  0x000be188
      Q[14] = (rng() & 0x1cf01e77) + 0x000be188;

      // Q[15] = v110 0001 ..V. .... 10.. .... .000 0000
      // RNG   = *... .... **** **** ..** **** *... ....  0x80ff3f80
      // 0     = ...* ***. .... .... .*.. .... .*** ****  0x1e00407f
      // 1     = .**. ...* .... .... *... .... .... ....  0x61008000
      Q[15] = (rng() & 0x80ff3f80) + 0x61008000;

      // Q[16] = ^010 00.. ..A. .... v... .... .000 v000
      // RNG   = .... ..** **.* **** **** **** *... *...  0x03dfff88
      // 0     = .*.* **.. .... .... .... .... .*** .***  0x5c000077
      // 1     = ..*. .... .... .... .... .... .... ....  0x20000000
      // Q[15] = *... .... .... .... .... .... .... ....  0x80000000
      // ~Q[15]= .... .... ..*. .... .... .... .... ....  0x00200000
      Q[16] =
        (rng() & 0x03dfff88) +
        0x20000000 +
        (Q[15] & 0x80000000) +
        (~Q[15] & 0x00200000);

      // Q[17] = ^1v. .... .... ..0. ^... .... .... ^...
      // RNG   = ..** **** **** **.* .*** **** **** .***  0x3ffd7ff7
      // 0     = .... .... .... ..*. .... .... .... ....  0x00020000
      // 1     = .*.. .... .... .... .... .... .... ....  0x40000000
      // Q[16] = *... .... .... .... *... .... .... *...  0x80008008
      Q[17] = (rng() & 0x3ffd7ff7) + 0x40000000 + (Q[16] & 0x80008008);

      // Start message creation
      x[0] = RR(Q[1] - QM0, 7) - F(QM0, QM1, QM2) - QM3 - 0xd76aa478;
      x[1] = RR(Q[17] - Q[16], 5) - G(Q[16], Q[15], Q[14]) - Q[13] - 0xf61e2562;
      0;
      x[4] = RR(Q[5] - Q[4], 7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0faf;
      x[5] = RR(Q[6] - Q[5], 12) - F(Q[5], Q[4], Q[3]) - Q[2] - 0x4787c62a;
      x[6] = RR(Q[7] - Q[6], 17) - F(Q[6], Q[5], Q[4]) - Q[3] - 0xa8304613;
      x[10] = RR(Q[11] - Q[10], 17) - F(Q[10], Q[9], Q[8]) - Q[7] - 0xffff5bb1;
      x[11] = RR(Q[12] - Q[11], 22) - F(Q[11], Q[10], Q[9]) - Q[8] - 0x895cd7be;
      x[15] =
        RR(Q[16] - Q[15], 22) - F(Q[15], Q[14], Q[13]) - Q[12] - 0x49b40821;

      // Q[2] = .... .... .... .... .... .... .... ....
      Q[2] = Q[1] + RL(F(Q[1], QM0, QM1) + QM2 + x[1] + 0xe8c7b756, 12);

      // Q[18] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[18] = Q[17] + RL(G(Q[17], Q[16], Q[15]) + Q[14] + x[6] + 0xc040b340, 9);

      // Q[17] = ^1v. .... .... ..0. ^... .... .... ^...
      // Q[18] = ^.^. .... .... ..1. .... .... .... ....
      //         1010 0000 0000 0010 0000 0000 0000 0000  0xa0020000
      if (((Q[18] ^ Q[17]) & 0xa0020000) != 0x00020000) continue;

      // Q[19] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ19,4 ~ Σ19,18 not all 1
      // 0x0003fff8 = 0000 0000 0000 0011 1111 1111 1111 1000
      sigma_Q19 = G(Q[18], Q[17], Q[16]) + Q[15] + x[11] + 0x265e5a51;
      if ((sigma_Q19 & 0x0003fff8) == 0x0003fff8) continue;

      Q[19] = Q[18] + RL(sigma_Q19, 14);

      // Q[18] = ^.^. .... .... ..1. .... .... .... ....
      // Q[19] = ^... .... .... ..0. .... .... .... ....
      //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000
      if (((Q[19] ^ Q[18]) & 0x80020000) != 0x00020000) {
        continue;
      }

      // Q[20] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ20,30 ~ Σ20,32 not all 0
      // 0xe0000000 = 1110 0000 0000 0000 0000 0000 0000 0000
      sigma_Q20 = G(Q[19], Q[18], Q[17]) + Q[16] + x[0] + 0xe9b6c7aa;
      if ((sigma_Q20 & 0xe0000000) == 0) continue;

      Q[20] = Q[19] + RL(sigma_Q20, 20);

      // Q[20] = ^... .... .... ..v. .... .... .... ....
      if (bit(Q[20], 32) != bit(Q[15], 32)) continue;

      // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[21] = Q[20] + RL(G(Q[20], Q[19], Q[18]) + Q[17] + x[5] + 0xd62f105d, 5);

      // Q[20] = ^... .... .... ..v. .... .... .... ....
      // Q[21] = ^... .... .... ..^. .... .... .... ....
      //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000
      if (((Q[21] ^ Q[20]) & 0x80020000) != 0) continue;

      // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[22] = Q[21] + RL(G(Q[21], Q[20], Q[19]) + Q[18] + x[10] + 0x2441453, 9);

      // Q[22] = ^... .... .... .... .... .... .... ....
      if (bit(Q[22], 32) != bit(Q[15], 32)) continue;

      // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ23,18 = 0
      sigma_Q23 = G(Q[22], Q[21], Q[20]) + Q[19] + x[15] + 0xd8a1e681;
      if (bit(sigma_Q23, 18) != 0) continue;

      Q[23] = Q[22] + RL(sigma_Q23, 14);

      // Q[23] = 0... .... .... .... .... .... .... ....
      if (bit(Q[23], 32) != 0) continue;

      // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[24] =
        Q[23] + RL(G(Q[23], Q[22], Q[21]) + Q[20] + x[4] + 0xe7d3fbc8, 20);

      // Q[24] = 1... .... .... .... .... .... .... ....
      if (bit(Q[24], 32) != 1) continue;
      // Every bit condition in Q[1]..Q[24] is now satisfied. We proceed with
      return {
        x,
        Q,
        it,
        seed: lastSeed,
      };
    }
    return null;
  }
}

function Block1(input, rngMask) {
  const IV1 = 0x67452301;
  const IV2 = 0xefcdab89;
  const IV3 = 0x98badcfe;
  const IV4 = 0x10325476;

  const startQ4 = (input && input.startQ4) || 0;
  const startQ9 = (input && input.startQ9) || 0;
  const startQ13 = (input && input.startQ13) || 0;
  const startQ10 = (input && input.startQ10) || 0;
  const startQ20 = (input && input.startQ20) || 0;
  const startQ14 = (input && input.startQ14) || 0;
  const seed = (input && input.seed) || X;
  // const Q = newArray(65);
  // const x = newArray(16);
  let sigma_Q23 = 0,
    sigma_Q35 = 0,
    sigma_Q62 = 0;
  let i = 0,
    itr_Q9 = 0,
    itr_Q4 = 0,
    itr_Q14 = 0,
    itr_Q13 = 0,
    itr_Q20 = 0,
    itr_Q10 = 0;
  let tmp_q3 = 0,
    tmp_q4 = 0,
    tmp_q13 = 0,
    tmp_q14 = 0,
    tmp_q20 = 0,
    tmp_q21 = 0,
    tmp_q9 = 0,
    tmp_q10 = 0;
  let tmp_x1 = 0,
    tmp_x15 = 0,
    tmp_x4 = 0;
  let Q3_fix = 0,
    Q4_fix = 0,
    Q14_fix = 0,
    const_masked = 0,
    const_unmasked = 0;
  let AA0 = 0,
    BB0 = 0,
    CC0 = 0,
    DD0 = 0,
    AA1 = 0,
    BB1 = 0,
    CC1 = 0,
    DD1 = 0;

  // Initialization vectors
  const QM3 = IV1;
  const QM0 = IV2;
  const QM1 = IV3;
  const QM2 = IV4;

  let candidateGenerator = new Block1CandidatesGenerator(seed, rngMask);

  // Start block 1 generation.
  // TO-DO: add a time limit for collision search.
  for (let it = 0; it < 1; it++) {
    let candidate = candidateGenerator.getnext(2);
    if (!candidate) {
      console.warn('no candidate found');
      return -1;
    }
    // console.log('seed', candidate.seed >>> 0);
    let { x, Q } = candidate;
    tmp_x1 = x[1];
    tmp_x4 = x[4];
    tmp_x15 = x[15];

    tmp_q3 = Q[3];
    tmp_q4 = Q[4];
    tmp_q9 = Q[9];
    tmp_q10 = Q[10];
    tmp_q13 = Q[13];
    tmp_q14 = Q[14];
    tmp_q20 = Q[20];
    tmp_q21 = Q[21];

    ///////////////////////////////////////////////////////////////
    ///                       Tunnel Q10                         //
    ///////////////////////////////////////////////////////////////
    // Tunnel Q10 - 3 bits - Probabilistic. Modifications on x[10] disturb
    // probabilistically conditions for Q[22-24]
    for (
      itr_Q10 = startQ10;
      itr_Q10 < (USE_B1_Q10 ? Math.pow(2, Q10_strength) : 1);
      itr_Q10++
    ) {
      Q[9] = tmp_q9;
      Q[10] = tmp_q10;
      Q[13] = tmp_q13;
      Q[20] = tmp_q20;
      Q[21] = tmp_q21;

      x[4] = tmp_x4;
      x[15] = tmp_x15;

      // Multi message modification - Q10 is modified according to its mask
      // (bits 11,25,27)
      Q[10] = tmp_q10 ^ mask_Q10[USE_B1_Q10 ? itr_Q10 : 0];

      // x[10] is modified and related states are regenerated
      x[10] = RR(Q[11] - Q[10], 17) - F(Q[10], Q[9], Q[8]) - Q[7] - 0xffff5bb1;

      // Q10 Tunnel - Verification of bit conditions on Q[22-24]

      // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[22] = Q[21] + RL(G(Q[21], Q[20], Q[19]) + Q[18] + x[10] + 0x2441453, 9);

      // Q[22] = ^... .... .... .... .... .... .... ....
      if (bit(Q[22], 32) != bit(Q[15], 32)) continue;

      // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ23,18 = 0
      sigma_Q23 = G(Q[22], Q[21], Q[20]) + Q[19] + x[15] + 0xd8a1e681;
      if (bit(sigma_Q23, 18) != 0) continue;

      Q[23] = Q[22] + RL(sigma_Q23, 14);

      // Q[23] = 0... .... .... .... .... .... .... ....
      if (bit(Q[23], 32) != 0) continue;

      // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[24] =
        Q[23] + RL(G(Q[23], Q[22], Q[21]) + Q[20] + x[4] + 0xe7d3fbc8, 20);

      // Q[24] = 1... .... .... .... .... .... .... ....
      if (bit(Q[24], 32) != 1) continue;

      ///////////////////////////////////////////////////////////////
      ///                       Tunnel Q20                         //
      ///////////////////////////////////////////////////////////////
      // Tunnel Q20 - 6 bits - Probabilistic. Modifications on Q[20] and free
      // choice of Q[1] and Q[2] lead to change in x[0] and x[2..5]
      for (
        itr_Q20 = startQ20;
        itr_Q20 < (USE_B1_Q20 ? Math.pow(2, Q20_strength) : 1);
        itr_Q20++
      ) {
        Q[3] = tmp_q3;
        Q[4] = tmp_q4;

        x[1] = tmp_x1;
        x[15] = tmp_x15;

        // Q20 is modified according to its mask (bits 1,2,10,15,22,24)
        Q[20] = tmp_q20 ^ mask_Q20[USE_B1_Q20 ? itr_Q20 : 0];

        x[0] =
          RR(Q[20] - Q[19], 20) - G(Q[19], Q[18], Q[17]) - Q[16] - 0xe9b6c7aa;

        Q[1] = QM0 + RL(F(QM0, QM1, QM2) + QM3 + x[0] + 0xd76aa478, 7);
        Q[2] = Q[1] + RL(F(Q[1], QM0, QM1) + QM2 + x[1] + 0xe8c7b756, 12);

        x[4] = RR(Q[5] - Q[4], 7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0faf;
        x[5] = RR(Q[6] - Q[5], 12) - F(Q[5], Q[4], Q[3]) - Q[2] - 0x4787c62a;

        // Tunnel Q20 - Verification of bit conditions on Q[21-24]
        // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[21] =
          Q[20] + RL(G(Q[20], Q[19], Q[18]) + Q[17] + x[5] + 0xd62f105d, 5);

        // Q[20] = ^... .... .... ..v. .... .... .... ....
        // Q[21] = ^... .... .... ..^. .... .... .... ....
        //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000
        if (((Q[21] ^ Q[20]) & 0x80020000) != 0) continue;

        // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[22] =
          Q[21] + RL(G(Q[21], Q[20], Q[19]) + Q[18] + x[10] + 0x2441453, 9);

        // Q[22] = ^... .... .... .... .... .... .... ....
        if (bit(Q[22], 32) != bit(Q[15], 32)) continue;

        // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Extra conditions: Σ23,18 = 0
        sigma_Q23 = G(Q[22], Q[21], Q[20]) + Q[19] + x[15] + 0xd8a1e681;
        if (bit(sigma_Q23, 18) != 0) continue;

        Q[23] = Q[22] + RL(sigma_Q23, 14);

        // Q[23] = 0... .... .... .... .... .... .... ....
        if (bit(Q[23], 32) != 0) continue;

        // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[24] =
          Q[23] + RL(G(Q[23], Q[22], Q[21]) + Q[20] + x[4] + 0xe7d3fbc8, 20);

        // Q[24] = 1... .... .... .... .... .... .... ....
        if (bit(Q[24], 32) != 1) continue;

        ///////////////////////////////////////////////////////////////
        ///                       Tunnel Q13                         //
        ///////////////////////////////////////////////////////////////
        // Tunnel Q13 - 12 bits - Probabilistic. Modifications on Q[13] and free
        // choice of Q[2] lead to change in x[1..5] and x[15]
        for (
          itr_Q13 = startQ13;
          itr_Q13 < (USE_B1_Q13 ? Math.pow(2, Q13_strength) : 1);
          itr_Q13++
        ) {
          Q[3] = tmp_q3;
          Q[4] = tmp_q4;
          Q[14] = tmp_q14;

          Q[13] = tmp_q13 ^ mask_Q13[USE_B1_Q13 ? itr_Q13 : 0];

          x[1] =
            RR(Q[17] - Q[16], 5) - G(Q[16], Q[15], Q[14]) - Q[13] - 0xf61e2562;

          Q[2] = Q[1] + RL(F(Q[1], QM0, QM1) + QM2 + x[1] + 0xe8c7b756, 12);

          x[4] = RR(Q[5] - Q[4], 7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0faf;
          x[5] = RR(Q[6] - Q[5], 12) - F(Q[5], Q[4], Q[3]) - Q[2] - 0x4787c62a;
          x[15] =
            RR(Q[16] - Q[15], 22) - F(Q[15], Q[14], Q[13]) - Q[12] - 0x49b40821;

          // Tunnel Q13 - Verification of bit conditions on Q[21-24]
          // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Q[21] =
            Q[20] + RL(G(Q[20], Q[19], Q[18]) + Q[17] + x[5] + 0xd62f105d, 5);

          // Q[20] = ^... .... .... ..v. .... .... .... ....
          // Q[21] = ^... .... .... ..^. .... .... .... ....
          //         1000 0000 0000 0010 0000 0000 0000 0000  0x80020000
          if (((Q[21] ^ Q[20]) & 0x80020000) != 0) continue;

          // Q[22] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Q[22] =
            Q[21] + RL(G(Q[21], Q[20], Q[19]) + Q[18] + x[10] + 0x2441453, 9);

          // Q[22] = ^... .... .... .... .... .... .... ....
          if (bit(Q[22], 32) != bit(Q[15], 32)) continue;

          // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          // Extra conditions: Σ23,18 = 0
          sigma_Q23 = G(Q[22], Q[21], Q[20]) + Q[19] + x[15] + 0xd8a1e681;
          if (bit(sigma_Q23, 18) != 0) continue;

          Q[23] = Q[22] + RL(sigma_Q23, 14);

          // Q[23] = 0... .... .... .... .... .... .... ....
          if (bit(Q[23], 32) != 0) continue;

          // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Q[24] =
            Q[23] + RL(G(Q[23], Q[22], Q[21]) + Q[20] + x[4] + 0xe7d3fbc8, 20);

          // Q[24] = 1... .... .... .... .... .... .... ....
          if (bit(Q[24], 32) != 1) continue;

          ///////////////////////////////////////////////////////////////
          ///                       Tunnel Q14                         //
          ///////////////////////////////////////////////////////////////
          // Tunnel Q14 - 9 bits - Dynamic tunnel. We will find bit positions i
          // where we can change Q[3][i] and/or Q[4][i] such that doesn't affect
          // x[5] in the equation for Q[6] In particular v =
          // F(Q[5][i],Q[4][i],Q[5][i]) has to remain unchanged. Is dynamic
          // because, according to the value of Q[5][i] we will decide which one
          // of the bits of Q[4][i],Q[5][i] will be changed to maintain v
          // unchanged. If we change both bits Q[4][i],Q[5][i] v will change.
          // Thus bit positions where we have sufficient conditions Q[3][i] =
          // Q[4][i] are useless (we want to change them to have MMM)

          // Bits for Q[3]
          //  The ones that has to remain unchanged or are useless
          //  0x77ffffda = 0111  0111  1111  1111  1111  1111  1101  1010
          //  The ones that can be changed
          //  0x88000025 = 1000  1000  0000  0000  0000  0000  0010  0101

          // Bits for Q[4]
          //  The ones that has to remain unchanged or are useless
          //  0x8bfffff5 = 1000  1011  1111  1111  1111  1111  1111  0101
          //  The ones that can be changed
          //  0x7400000a = 0111  0100  0000  0000  0000  0000  0000  1010

          // Bits for Q[14]
          //  The ones that has to remain unchanged or are useless. (the
          //  not-in-mask bits) 0xe3ffff88 = 1110  0011  1111  1111  1111  1111
          //  1000  1000 The ones that can be changed (mask bits) 0x7400000a =
          //  0001  1100  0000  0000  0000  0000  0111  0111

          // Summarizing, bits that can be changed in Q[3]/Q[4] are the XOR
          //  0x88000025 = 1000  1000  0000  0000  0000  0000  0010  0101
          //     XOR
          //  0x7400000a = 0111  0100  0000  0000  0000  0000  0000  1010
          //  -----------------------------------------------------------
          //  0xfc00002f = 1111  1100  0000  0000  0000  0000  0010  1111
          //               \______/                              |___\__/
          //                 (1)                                    (2)
          //
          // Change in bits 1,2,3,5,6,7 for Q[14] will affect bits in (1)
          // Change in bits 27,28,29 for Q[14] will affect bits in (2)

          // Unchanged bits of Q[3],Q[4],Q[14]
          Q3_fix = Q[3] & 0x77ffffda;
          Q4_fix = Q[4] & 0x8bfffff5;
          Q14_fix = Q[14] & 0xe3ffff88;

          // Relation for Q[18], Q[7]:
          //  Q[18] = Q[17]+RL(G(Q[17],Q[16],Q[15])+Q[14]+x[ 6]+0xc040b340, 9);
          //  Q[ 7] = Q[ 6]+RL(F(Q[ 6],Q[ 5],Q[ 4])+Q[ 3]+x[ 6]+0xa8304613,17);

          // We eliminate x[6] from our equations. We will work on
          // const_unmasked to compensate variations on bits
          const_unmasked =
            RR(Q[7] - Q[6], 17) -
            0xa8304613 - //= F(Q[ 6],Q[ 5],Q[ 4]) + Q[ 3] + x[ 6]
            RR(Q[18] - Q[17], 9) +
            G(Q[17], Q[16], Q[15]) +
            0xc040b340 - //= -Q[14] -x[ 6]
            F(Q[6], Q[5], Q4_fix) -
            Q3_fix +
            Q14_fix;

          // So const_unmasked is the difference (F(Q[6],Q[5],Q[4]) -
          // F(Q[6],Q[5],Q4_fix)) + (Q[3]-Q3_fix) - (Q[14]-Q14_fix)

          // From const_unmasked, that depends on the current value for Q[5],
          // we'll get the new values for Q[3],Q[4] (i.e. the bits that we have
          // to change to don't affect x[5])

          // Tunnel Q14 starts
          for (
            itr_Q14 = startQ14;
            itr_Q14 < (USE_B1_Q14 ? Math.pow(2, Q14_strength) : 1);
            itr_Q14++
          ) {
            // console.log("Q14", itr_Q14);
            // Q14 is modified according to its mask {1, 2, 3, 5, 6, 7, 27, 28,
            // 29} NOTE that const_unmasked consider carries. So operations are
            // +,- and not XOR.
            const_masked = const_unmasked + mask_Q14[USE_B1_Q14 ? itr_Q14 : 0];

            // If the current value for Q[14] affects bits in const_masked that
            // are outside 0xfc00002f = 1111  1100  0000  0000  0000  0000  0010
            // 1111 this means that this modification cannot be compensated by
            // Q[3]/Q[4] and then we need to continue
            //
            // 0x03ffffd0 = 0000  0011  1111  1111  1111  1111  1101  0000
            if ((const_masked & 0x03ffffd0) != 0) continue;

            // We recover the remaining bits of Q[3],Q[4] and Q[14] from the
            // current const_masked
            Q[3] = Q3_fix + (const_masked & 0x88000025);
            Q[4] = Q4_fix + (const_masked & 0x7400000a);
            Q[14] = Q14_fix + mask_Q14[itr_Q14];

            x[2] = RR(Q[3] - Q[2], 17) - F(Q[2], Q[1], QM0) - QM1 - 0x242070db;

            ///////////////////////////////////////////////////////////////
            ///                       Tunnel Q4                          //
            ///////////////////////////////////////////////////////////////
            // Tunnel Q4 - 1 bit - Probabilistic tunnel. Modification on
            // Q[4][26] will probably affect Q[24][32]
            for (
              itr_Q4 = startQ4;
              itr_Q4 < (USE_B1_Q4 ? Math.pow(2, Q4_strength) : 1);
              itr_Q4++
            ) {
              Q[4] = Q[4] ^ mask_Q4[USE_B1_Q4 ? itr_Q4 : 0];

              x[4] =
                RR(Q[5] - Q[4], 7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0faf;

              // Q[24] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
              Q[24] =
                Q[23] +
                RL(G(Q[23], Q[22], Q[21]) + Q[20] + x[4] + 0xe7d3fbc8, 20);

              // Q[24] = 1... .... .... .... .... .... .... ....
              if (bit(Q[24], 32) != 1) continue;

              x[3] =
                RR(Q[4] - Q[3], 22) - F(Q[3], Q[2], Q[1]) - QM0 - 0xc1bdceee;
              x[6] =
                RR(Q[7] - Q[6], 17) - F(Q[6], Q[5], Q[4]) - Q[3] - 0xa8304613;
              x[7] =
                RR(Q[8] - Q[7], 22) - F(Q[7], Q[6], Q[5]) - Q[4] - 0xfd469501;
              x[13] =
                RR(Q[14] - Q[13], 12) -
                F(Q[13], Q[12], Q[11]) -
                Q[10] -
                0xfd987193;
              x[14] =
                RR(Q[15] - Q[14], 17) -
                F(Q[14], Q[13], Q[12]) -
                Q[11] -
                0xa679438e;

              ///////////////////////////////////////////////////////////////
              ///                       Tunnel Q9                          //
              ///////////////////////////////////////////////////////////////
              // Tunnel Q9 - 3 bits - Deterministic tunnel. If the i-th bit of
              // Q[10] would be zero and the i-th bit of Q[11] would be one, an
              // eventual change of the i-th bit of Q[9] shouldn't affect the
              // equations for Q[11] and Q[12].
              for (
                itr_Q9 = startQ9;
                itr_Q9 < (USE_B1_Q9 ? Math.pow(2, Q9_strength) : 1);
                itr_Q9++
              ) {
                Q[9] = tmp_q9 ^ mask_Q9[USE_B1_Q9 ? itr_Q9 : 0];

                x[8] =
                  RR(Q[9] - Q[8], 7) - F(Q[8], Q[7], Q[6]) - Q[5] - 0x698098d8;
                x[9] =
                  RR(Q[10] - Q[9], 12) -
                  F(Q[9], Q[8], Q[7]) -
                  Q[6] -
                  0x8b44f7af;
                x[12] =
                  RR(Q[13] - Q[12], 7) -
                  F(Q[12], Q[11], Q[10]) -
                  Q[9] -
                  0x6b901122;

                Q[25] =
                  Q[24] +
                  RL(G(Q[24], Q[23], Q[22]) + Q[21] + x[9] + 0x21e1cde6, 5);
                Q[26] =
                  Q[25] +
                  RL(G(Q[25], Q[24], Q[23]) + Q[22] + x[14] + 0xc33707d6, 9);
                Q[27] =
                  Q[26] +
                  RL(G(Q[26], Q[25], Q[24]) + Q[23] + x[3] + 0xf4d50d87, 14);
                Q[28] =
                  Q[27] +
                  RL(G(Q[27], Q[26], Q[25]) + Q[24] + x[8] + 0x455a14ed, 20);
                Q[29] =
                  Q[28] +
                  RL(G(Q[28], Q[27], Q[26]) + Q[25] + x[13] + 0xa9e3e905, 5);
                Q[30] =
                  Q[29] +
                  RL(G(Q[29], Q[28], Q[27]) + Q[26] + x[2] + 0xfcefa3f8, 9);
                Q[31] =
                  Q[30] +
                  RL(G(Q[30], Q[29], Q[28]) + Q[27] + x[7] + 0x676f02d9, 14);
                Q[32] =
                  Q[31] +
                  RL(G(Q[31], Q[30], Q[29]) + Q[28] + x[12] + 0x8d2a4c8a, 20);
                Q[33] =
                  Q[32] +
                  RL(H(Q[32], Q[31], Q[30]) + Q[29] + x[5] + 0xfffa3942, 4);
                Q[34] =
                  Q[33] +
                  RL(H(Q[33], Q[32], Q[31]) + Q[30] + x[8] + 0x8771f681, 11);

                // Extra conditions: Σ35,16 = 0
                sigma_Q35 = H(Q[34], Q[33], Q[32]) + Q[31] + x[11] + 0x6d9d6122;
                if (bit(sigma_Q35, 16) != 0) continue;

                Q[35] = Q[34] + RL(sigma_Q35, 16);

                Q[36] =
                  Q[35] +
                  RL(H(Q[35], Q[34], Q[33]) + Q[32] + x[14] + 0xfde5380c, 23);
                Q[37] =
                  Q[36] +
                  RL(H(Q[36], Q[35], Q[34]) + Q[33] + x[1] + 0xa4beea44, 4);
                Q[38] =
                  Q[37] +
                  RL(H(Q[37], Q[36], Q[35]) + Q[34] + x[4] + 0x4bdecfa9, 11);
                Q[39] =
                  Q[38] +
                  RL(H(Q[38], Q[37], Q[36]) + Q[35] + x[7] + 0xf6bb4b60, 16);
                Q[40] =
                  Q[39] +
                  RL(H(Q[39], Q[38], Q[37]) + Q[36] + x[10] + 0xbebfbc70, 23);
                Q[41] =
                  Q[40] +
                  RL(H(Q[40], Q[39], Q[38]) + Q[37] + x[13] + 0x289b7ec6, 4);
                Q[42] =
                  Q[41] +
                  RL(H(Q[41], Q[40], Q[39]) + Q[38] + x[0] + 0xeaa127fa, 11);
                Q[43] =
                  Q[42] +
                  RL(H(Q[42], Q[41], Q[40]) + Q[39] + x[3] + 0xd4ef3085, 16);
                Q[44] =
                  Q[43] +
                  RL(H(Q[43], Q[42], Q[41]) + Q[40] + x[6] + 0x04881d05, 23);
                Q[45] =
                  Q[44] +
                  RL(H(Q[44], Q[43], Q[42]) + Q[41] + x[9] + 0xd9d4d039, 4);
                Q[46] =
                  Q[45] +
                  RL(H(Q[45], Q[44], Q[43]) + Q[42] + x[12] + 0xe6db99e5, 11);
                Q[47] =
                  Q[46] +
                  RL(H(Q[46], Q[45], Q[44]) + Q[43] + x[15] + 0x1fa27cf8, 16);
                Q[48] =
                  Q[47] +
                  RL(H(Q[47], Q[46], Q[45]) + Q[44] + x[2] + 0xc4ac5665, 23);

                // Sufficient conditions
                if (bit(Q[46], 32) != bit(Q[48], 32)) continue;

                Q[49] =
                  Q[48] +
                  RL(fI(Q[48], Q[47], Q[46]) + Q[45] + x[0] + 0xf4292244, 6);

                if (bit(Q[47], 32) != bit(Q[49], 32)) continue;

                Q[50] =
                  Q[49] +
                  RL(fI(Q[49], Q[48], Q[47]) + Q[46] + x[7] + 0x432aff97, 10);

                if (bit(Q[50], 32) != (bit(Q[48], 32) ^ 1)) continue;

                Q[51] =
                  Q[50] +
                  RL(fI(Q[50], Q[49], Q[48]) + Q[47] + x[14] + 0xab9423a7, 15);

                if (bit(Q[51], 32) != bit(Q[49], 32)) continue;

                Q[52] =
                  Q[51] +
                  RL(fI(Q[51], Q[50], Q[49]) + Q[48] + x[5] + 0xfc93a039, 21);

                if (bit(Q[52], 32) != bit(Q[50], 32)) continue;

                Q[53] =
                  Q[52] +
                  RL(fI(Q[52], Q[51], Q[50]) + Q[49] + x[12] + 0x655b59c3, 6);

                if (bit(Q[53], 32) != bit(Q[51], 32)) continue;

                Q[54] =
                  Q[53] +
                  RL(fI(Q[53], Q[52], Q[51]) + Q[50] + x[3] + 0x8f0ccc92, 10);

                if (bit(Q[54], 32) != bit(Q[52], 32)) continue;

                Q[55] =
                  Q[54] +
                  RL(fI(Q[54], Q[53], Q[52]) + Q[51] + x[10] + 0xffeff47d, 15);

                if (bit(Q[55], 32) != bit(Q[53], 32)) continue;

                Q[56] =
                  Q[55] +
                  RL(fI(Q[55], Q[54], Q[53]) + Q[52] + x[1] + 0x85845dd1, 21);

                if (bit(Q[56], 32) != bit(Q[54], 32)) continue;

                Q[57] =
                  Q[56] +
                  RL(fI(Q[56], Q[55], Q[54]) + Q[53] + x[8] + 0x6fa87e4f, 6);

                if (bit(Q[57], 32) != bit(Q[55], 32)) continue;

                Q[58] =
                  Q[57] +
                  RL(fI(Q[57], Q[56], Q[55]) + Q[54] + x[15] + 0xfe2ce6e0, 10);

                if (bit(Q[58], 32) != bit(Q[56], 32)) continue;

                Q[59] =
                  Q[58] +
                  RL(fI(Q[58], Q[57], Q[56]) + Q[55] + x[6] + 0xa3014314, 15);

                if (bit(Q[59], 32) != bit(Q[57], 32)) continue;

                Q[60] =
                  Q[59] +
                  RL(fI(Q[59], Q[58], Q[57]) + Q[56] + x[13] + 0x4e0811a1, 21);

                if (bit(Q[60], 26) != 0) continue;

                if (bit(Q[60], 32) != (bit(Q[58], 32) ^ 1)) continue;

                Q[61] =
                  Q[60] +
                  RL(fI(Q[60], Q[59], Q[58]) + Q[57] + x[4] + 0xf7537e82, 6);

                if (bit(Q[61], 26) != 1) continue;

                if (bit(Q[61], 32) != bit(Q[59], 32)) continue;

                // Extra conditions: Σ62,16 ~ Σ62,22 not all ones
                // 0x003f8000 = 0000  0000  0011  1111  1000  0000  0000  0000
                sigma_Q62 =
                  fI(Q[61], Q[60], Q[59]) + Q[58] + x[11] + 0xbd3af235;
                if ((sigma_Q62 & 0x003f8000) == 0x003f8000) continue;

                Q[62] = Q[61] + RL(sigma_Q62, 10);

                Q[63] =
                  Q[62] +
                  RL(fI(Q[62], Q[61], Q[60]) + Q[59] + x[2] + 0x2ad7d2bb, 15);
                Q[64] =
                  Q[63] +
                  RL(fI(Q[63], Q[62], Q[61]) + Q[60] + x[9] + 0xeb86d391, 21);

                // We add the initial vector to obtain the Intermediate Hash
                // Values of the current block
                AA0 = IV1 + Q[61];
                BB0 = IV2 + Q[64];
                CC0 = IV3 + Q[63];
                DD0 = IV4 + Q[62];

                // Last sufficient conditions
                if (bit(BB0, 6) != 0) continue;

                if (bit(BB0, 26) != 0) continue;

                if (bit(BB0, 27) != 0) continue;

                if (bit(CC0, 26) != 1) continue;

                if (bit(CC0, 27) != 0) continue;

                if (bit(DD0, 26) != 0) continue;

                if (bit(BB0, 32) != bit(CC0, 32)) continue;

                if (bit(CC0, 32) != bit(DD0, 32)) continue;

                // Message 1 block 1 computation completed.

                // Now we see if the differential path is verified
                // Note that message 1 block 1 is x = x[0]||...||x[15]
                // While message 2 block 1 is Hx = x + C

                // Message 2 block 1 hash computation
                let obj = createMD5Object();
                let { Hx } = obj;
                for (i = 0; i < 16; i++) Hx[i] = x[i];

                Hx[4] = x[4] + 0x80000000;
                Hx[11] = x[11] + 0x00008000;
                Hx[14] = x[14] + 0x80000000;

                // We set the IV, hash Hx and get the Intermediate Hash Value
                obj.a = IV1 >>> 0;
                obj.b = IV2 >>> 0;
                obj.c = IV3 >>> 0;
                obj.d = IV4 >>> 0;

                obj = HMD5Tr(obj);

                AA1 = (IV1 + obj.a) >>> 0;
                BB1 = (IV2 + obj.b) >>> 0;
                CC1 = (IV3 + obj.c) >>> 0;
                DD1 = (IV4 + obj.d) >>> 0;
                // console.log(AA1, BB1, CC1, DD1);

                // We see if the Differential Path is verified,
                if (
                  (AA1 - AA0) >>> 0 != 0x80000000 ||
                  (BB1 - BB0) >>> 0 != 0x82000000 ||
                  (CC1 - CC0) >>> 0 != 0x82000000 ||
                  (DD1 - DD0) >>> 0 != 0x82000000
                )
                  continue;

                // We store the intermediate hash values
                const A0 = AA0;
                const B0 = BB0;
                const C0 = CC0;
                const D0 = DD0;
                const A1 = AA1;
                const B1 = BB1;
                const C1 = CC1;
                const D1 = DD1;

                // We store both first blocks
                for (i = 0; i < 16; i++) {
                  memcpy(v1, 4 * i, x, i, 4);
                  memcpy(v2, 4 * i, Hx, i, 4);
                }
                return {
                  v1,
                  v2,
                  A0,
                  B0,
                  C0,
                  D0,
                  A1,
                  B1,
                  C1,
                  D1,
                  X: candidateGenerator.X
                };
              } // End of Q9 Tunnel
            } // End of Q4 Tunnel
          } // End of Q14 Tunnel
        } // End of Q13 Tunnel
      } // End of Q20 Tunnel
    } // End of Q10 Tunnel
  } // End of general for
  return undefined; // Collision not found;
}
