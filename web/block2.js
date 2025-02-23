class Block2Generator {
  constructor(rngMask) {
    this.Q = newArray(65);
    this.x = newArray(16);
    this.Q9_mask_bits = [3, 4, 5, 11, 19, 21, 22, 23];
    this.Q9_strength = 8;
    this.mask_Q9 = generate_mask(this.Q9_strength, this.Q9_mask_bits);

    // Mask generation for MMMM Q4 - 6 bits
    this.Q4_mask_bits = [14, 15, 16, 23, 24, 25];
    this.Q4_strength = 6;
    this.mask_Q4 = generate_mask(this.Q4_strength, this.Q4_mask_bits);
    this.rngMask = rngMask;
  }

  rng() {
    this.X = (1664525 * this.X + 1013904223) & 0xffffffff;
    //X = (((((1103515245 >>> 0) * X >>>0) + 12345)>>>0) & 0xffffffff) >>> 0;

    return this.X & this.rngMask;
  }

  initBlock1(block1) {
    this.A0 = block1.A0;
    this.B0 = block1.B0;
    this.C0 = block1.C0;
    this.D0 = block1.D0;
    this.A1 = block1.A1;
    this.B1 = block1.B1;
    this.C1 = block1.C1;
    this.D1 = block1.D1;
    this.v1 = block1.v1;
    this.v2 = block1.v2;
  }

  iteration(X) {
    this.X = X;
    this.QM3 = this.A0;
    this.QM0 = this.B0;
    this.QM1 = this.C0;
    this.QM2 = this.D0;
    const QM0 = this.QM0;
    const QM1 = this.QM1;
    const Q = this.Q;
    const rng = this.rng.bind(this);
    const I = QM0 & 0x80000000;
    const not_I = ~QM0 & 0x80000000;
    // Q[ 1] = ~Ivvv  010v  vv1v  vvv1  .vvv  0vvv  vv0.  ...v
    // RNG   =  .***  ...*  **.*  ***.  ****  .***  **.*  ****  0x71def7df
    // 0     =  ....  *.*.  ....  ....  ....  *...  ..*.  ....  0x0a000820
    // 1     =  ....  .*..  ..*.  ...*  ....  ....  ....  ....  0x04210000
    Q[1] = (rng() & 0x71def7df) + 0x04210000 + not_I;

    // Multi message modif. meth. (MMMM) Q1Q2, Klima
    // Q[ 2] = ~I^^^  110^  ^^0^  ^^^1  0^^^  1^^^  ^^0v  v00^
    // RNG   =  ....  ....  ....  ....  ....  ....  ...*  *...  0x00000018
    // 0     =  ....  ..*.  ..*.  ....  *...  ....  ..*.  .**.  0x02208026
    // 1     =  ....  **..  ....  ...*  ....  *...  ....  ....  0x0c010800
    // Q[ 1] =  .***  ...*  **.*  ***.  .***  .***  **..  ...*  0x71de77c1
    Q[2] = (rng() & 0x00000018) + 0x0c010800 + (Q[1] & 0x71de77c1) + not_I;

    // Q[ 3] = ~I011  111.  ..01  1111  1..0  1vv1  011^  ^111
    // RNG   =  ....  ...*  **..  ....  .**.  .**.  ....  ....  0x01c06600
    // 0     =  .*..  ....  ..*.  ....  ...*  ....  *...  ....  0x40201080
    // 1     =  ..**  ***.  ...*  ****  *...  *..*  .**.  .***  0x3e1f8967
    // Q[ 2] =  ....  ....  ....  ....  ....  ....  ...*  *...  0x00000018
    Q[3] = (rng() & 0x01c06600) + 0x3e1f8967 + (Q[2] & 0x00000018) + not_I;

    // Q[ 4] = ~I011  101.  ..00  0100  ...0  0^^0  0001  0001
    // RNG   =  ....  ...*  **..  ....  ***.  ....  ....  ....  0x01c0e000
    // 0     =  .*..  .*..  ..**  *.**  ...*  *..*  ***.  ***.  0x443b19ee
    // 1     =  ..**  *.*.  ....  .*..  ....  ....  ...*  ...*  0x3a040011
    // Q[ 3] =  ....  ....  ....  ....  ....  .**.  ....  ....  0x00000600
    Q[4] = (rng() & 0x01c0e000) + 0x3a040011 + (Q[3] & 0x00000600) + not_I;

    // Q4 tunnel, Klima, bits 25-23,16-14
    // Q[ 5] =  I100  10.0  0010  1111  0000  1110  0101  0000
    // RNG   =  ....  ..*.  ....  ....  ....  ....  ....  ....  0x02000000
    // 0     =  ..**  .*.*  **.*  ....  ****  ...*  *.*.  ****  0x35d0f1af
    // 1     =  .*..  *...  ..*.  ****  ....  ***.  .*.*  ....  0x482f0e50
    Q[5] = (rng() & 0x02000000) + 0x482f0e50 + I;

    // Q4 tunnel, Klima, bits 25-23,16-14
    // Q[ 6] =  I..0  0101  1110  ..10  1110  1100  0101  0110
    // RNG   =  .**.  ....  ....  **..  ....  ....  ....  ....  0x600c0000
    // 0     =  ...*  *.*.  ...*  ...*  ...*  ..**  *.*.  *..*  0x1a1113a9
    // 1     =  ....  .*.*  ***.  ..*.  ***.  **..  .*.*  .**.  0x05e2ec56
    Q[6] = (rng() & 0x600c0000) + 0x05e2ec56 + I;

    // Q[ 7] = ~I..1  0111  1.00  ..01  10.1  1110  00..  ..v1
    // RNG   =  .**.  ....  .*..  **..  ..*.  ....  ..**  ***.  0x604c203e
    // 0     =  ....  *...  ..**  ..*.  .*..  ...*  **..  ....  0x083241c0
    // 1     =  ...*  .***  *...  ...*  *..*  ***.  ....  ...*  0x17819e01
    Q[7] = (rng() & 0x604c203e) + 0x17819e01 + not_I;

    // Q[ 8] = ~I..0  0100  0.11  ..10  1..v  ..11  111.  ..^0
    // RNG   =  .**.  ....  .*..  **..  .***  **..  ...*  **..  0x604c7c1c
    // 0     =  ...*  *.**  *...  ...*  ....  ....  ....  ...*  0x1b810001
    // 1     =  ....  .*..  ..**  ..*.  *...  ..**  ***.  ....  0x043283e0
    // Q[ 7] =  ....  ....  ....  ....  ....  ....  ....  ..*.  0x00000002
    Q[8] = (rng() & 0x604c7c1c) + 0x043283e0 + (Q[7] & 0x00000002) + not_I;

    // Q9 tunnel plus MMMM-Q12Q11, Klima, prepared, not programmed
    // Q[ 9] = ~Ivv1  1100  0xxx  .x01  0..^  .x01  110x  xx01
    // RNG   =  .**.  ....  .***  **..  .**.  **..  ...*  **..  0x607c6c1c
    // 0     =  ....  ..**  *...  ..*.  *...  ..*.  ..*.  ..*.  0x03828222
    // 1     =  ...*  **..  ....  ...*  ....  ...*  **..  ...*  0x1c0101c1
    // Q[ 8] =  ....  ....  ....  ....  ...*  ....  ....  ....  0x00001000
    Q[9] = (rng() & 0x607c6c1c) + 0x1c0101c1 + (Q[8] & 0x00001000) + not_I;

    // Q9 tunnel plus MMMM-Q12Q11, Klima
    // Q[10] = ~I^^1  1111  1000  v011  1vv0  1011  1100  0000
    // RNG   =  ....  ....  ....  *...  .**.  ....  ....  ....  0x00086000
    // 0     =  ....  ....  .***  .*..  ...*  .*..  ..**  ****  0x0074143f
    // 1     =  ...*  ****  *...  ..**  *...  *.**  **..  ....  0x1f838bc0
    // Q[ 9] =  .**.  ....  ....  ....  ....  ....  ....  ....  0x60000000
    Q[10] = (rng() & 0x00086000) + 0x1f838bc0 + (Q[9] & 0x60000000) + not_I;

    // Q9 tunnel plus MMMM-Q12Q11, Klima
    // Q[11] = ~Ivvv  vvvv  .111  ^101  1^^0  0111  11v1  1111
    // RNG   =  .***  ****  *...  ....  ....  ....  ..*.  ....  0x7f800020
    // 0     =  ....  ....  ....  ..*.  ...*  *...  ....  ....  0x00021800
    // 1     =  ....  ....  .***  .*.*  *...  .***  **.*  ****  0x007587df
    // Q[10] =  ....  ....  ....  *...  .**.  ....  ....  ....  0x00086000
    Q[11] = (rng() & 0x7f800020) + 0x007587df + (Q[10] & 0x00086000) + not_I;

    // MMMM-Q12Q11, Klima
    // Q[12] = ~I^^^  ^^^^  ....  1000  0001  ....  1.^.  ....
    // RNG   =  ....  ....  ****  ....  ....  ****  .*.*  ****  0x00f00f5f
    // 0     =  ....  ....  ....  .***  ***.  ....  ....  ....  0x0007e000
    // 1     =  ....  ....  ....  *...  ...*  ....  *...  ....  0x00081080
    // Q[11] =  .***  ****  ....  ....  ....  ....  ..*.  ....  0x7f000020
    Q[12] = (rng() & 0x00f00f5f) + 0x00081080 + (Q[11] & 0x7f000020) + not_I;

    // Q[13] =  I011  1111  0...  1111  111.  ....  0...  1...
    // RNG   =  ....  ....  .***  ....  ...*  ****  .***  .***  0x00701f77
    // 0     =  .*..  ....  *...  ....  ....  ....  *...  ....  0x40800080
    // 1     =  ..**  ****  ....  ****  ***.  ....  ....  *...  0x3f0fe008
    Q[13] = (rng() & 0x00701f77) + 0x3f0fe008 + I;

    // Q[14] =  I100  0000  1...  1011  111.  ....  1...  1...
    // RNG   =  ....  ....  .***  ....  ...*  ****  .***  .***  0x00701f77
    // 0     =  ..**  ****  ....  .*..  ....  ....  ....  ....  0x3f040000
    // 1     =  .*..  ....  *...  *.**  ***.  ....  *...  *...  0x408be088
    Q[14] = (rng() & 0x00701f77) + 0x408be088 + I;

    // Next sufficient conditions until Q[24]:
    // Q[15] =  0111  1101  ....  ..10  00..  ....  ....  0...
    // Q[16] =  ^.10  ....  ....  ..01  1...  ....  ....  1...
    // Q[17] =  ^.v.  ....  ....  ..0.  1...  ....  ....  1...
    // Q[18] =  ^.^.  ....  ....  ..1.  ....  ....  ....  ....
    // Q[19] =  ^...  ....  ....  ..0.  ....  ....  ....  ....
    // Q[20] =  ^...  ....  ....  ..v.  ....  ....  ....  ....
    // Q[21] =  ^...  ....  ....  ..^.  ....  ....  ....  ....
    // Q[22] =  ^...  ....  ....  ....  ....  ....  ....  ....
    // Q[23] =  0...  ....  ....  ....  ....  ....  ....  ....
    // Q[24] =  1...  ....  ....  ....  ....  ....  ....  ....

    // In MMMM-Q[1]/Q[2] we want to change the value of x[0] without updating
    // the value of x[1], as updating x[1] will cause conditions on Q[17] not
    // hold. According to F, if QM0[i] = QM1[i] randomly choose the value where
    // Q[1][i] = Q[2][i] will not change the value of F(Q[1],QM0,QM1). We select
    // these bits
    //  Q[ 1] = ~Ivvv  010v  vv1v  vvv1  .vvv  0vvv  vv0.  ...v
    //  Q[ 2] = ~I^^^  110^  ^^0^  ^^^1  0^^^  1^^^  ^^0v  v00^
    //           0111  0001  1101  1110  0111  0111  1100  0001 = 0x71de77c1
    //
    // Note that (~(QM0 ^ QM1)) are all the bits where QM0[i] = QM1[i] and so
    // mask_Q1Q2 are all the bits where QM0[i] = QM1[i] and where Q[1][i] =
    // Q[2][i]. These bits will be changed.
    this.mask_Q1Q2 = ~(QM0 ^ QM1) & 0x71de77c1;
    this.Q1Q2_strength = 0;
    for (let i = 1; i < 33; i++) this.Q1Q2_strength += bit(this.mask_Q1Q2, i);

    this.Q1_fix = Q[1] & ~this.mask_Q1Q2;
    this.Q2_fix = Q[2] & ~this.mask_Q1Q2;

    this.tmp_q1 = Q[1];
    this.tmp_q2 = Q[2];
    this.tmp_q4 = Q[4];
    this.tmp_q9 = Q[9];
    this.doneStep = 0;
  }

  step2(NUM_BITS_Q16) {
    const QM3 = this.QM3;
    const QM0 = this.QM0;
    const QM1 = this.QM1;
    const QM2 = this.QM2;
    const rng = this.rng.bind(this);
    const Q = this.Q;
    const x = this.x;
    if (this.doneStep >= Math.pow(2, 20)) {
      console.warn("giving up on the second block");
      return false;
    }
    for (let itr_q16 = 0; itr_q16 < Math.pow(2, NUM_BITS_Q16); itr_q16++) {
      this.doneStep += 1;
      Q[1] = this.tmp_q1;
      Q[2] = this.tmp_q2;
      Q[4] = this.tmp_q4;
      Q[9] = this.tmp_q9;

      // Conditions by Liang-Lai says: Q[15] = (rng() & 0x80fc3ff7) +
      // 0x7d020000, Q[15] =  0111  1101  ....  ..10  00..  ....  ....  0... RNG
      // =  ....  ....  ****  **..  ..**  ****  ****  .***  0x80fc3ff7 0     =
      // *...  ..*.  ....  ...*  **..  ....  ....  *...  0x0201c008 1     = .***
      // **.*  ....  ..*.  ....  ....  ....  ....  0x7d020000
      Q[15] = (rng() & 0x00fc3ff7) + 0x7d020000;

      // Q[16] =  ^.10  ....  ....  ..01  1...  ....  ....  1...
      // RNG   =  .*..  ****  ****  **..  .***  ****  ****  .***  0x4ffc7ff7
      // 0     =  ...*  ....  ....  ..*.  ....  ....  ....  ....  0x10020000
      // 1     =  ..*.  ....  ....  ...*  *...  ....  ....  *...  0x20018008
      // Q[15] =  *.... ..... ..... .... ..... ..... ...... ....  0x80000000
      Q[16] = (rng() & 0x4ffc7ff7) + 0x20018008 + (Q[15] & 0x80000000);

      x[1] = RR(Q[2] - Q[1], 12) - F(Q[1], QM0, QM1) - QM2 - 0xe8c7b756;
      x[6] = RR(Q[7] - Q[6], 17) - F(Q[6], Q[5], Q[4]) - Q[3] - 0xa8304613;
      x[11] = RR(Q[12] - Q[11], 22) - F(Q[11], Q[10], Q[9]) - Q[8] - 0x895cd7be;

      // Q[17] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ17,25 ~ Σ17,27 not all 1
      // 0x07000000 =  0000 0111 0000 0000 0000 0000 0000 0000
      const sigma_Q17 = G(Q[16], Q[15], Q[14]) + Q[13] + x[1] + 0xf61e2562;
      if ((sigma_Q17 & 0x07000000) == 0x07000000) continue;

      // Q[16] =  ^.10  ....  ....  ..01  1...  ....  ....  1...
      // Q[17] =  ^.v.  ....  ....  ..0.  1...  ....  ....  1...
      //          1000  0000  0000  0010  1000  0000  0000  1000 0x80028008
      Q[17] = Q[16] + RL(sigma_Q17, 5);

      if ((Q[17] & 0x80028008) != (Q[16] & 0x80028008)) continue;

      // Q[18] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[18] = Q[17] + RL(G(Q[17], Q[16], Q[15]) + Q[14] + x[6] + 0xc040b340, 9);

      // Q[18] =  ^.^.  ....  ....  ..1.  ....  ....  ....  ....
      if (bit(Q[18], 18) != 1) continue;

      if ((Q[18] & 0xa0000000) != (Q[17] & 0xa0000000)) continue;

      // Q[19] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ19,4 ~ Σ19,18 not all 1
      // 0x0003fff8 =  0000 0000 0000 0011 1111 1111 1111 1000
      const sigma_Q19 = G(Q[18], Q[17], Q[16]) + Q[15] + x[11] + 0x265e5a51;
      if ((sigma_Q19 & 0x0003fff8) == 0x0003fff8) continue;

      Q[19] = Q[18] + RL(sigma_Q19, 14);

      // Q[19] =  ^...  ....  ....  ..0.  ....  ....  ....  ....
      if (bit(Q[19], 18) != 0) continue;

      if (bit(Q[19], 32) != bit(Q[18], 32)) continue;

      x[10] = RR(Q[11] - Q[10], 17) - F(Q[10], Q[9], Q[8]) - Q[7] - 0xffff5bb1;
      x[15] =
        RR(Q[16] - Q[15], 22) - F(Q[15], Q[14], Q[13]) - Q[12] - 0x49b40821;

      ///////////////////////////////////////////////////////////////
      ///                      MMMM Q1/Q2                          //
      ///////////////////////////////////////////////////////////////
      // MMMM Q1/Q2 - variable bits
      for (
        let itr_q1q2 = 0;
        itr_q1q2 < Math.pow(2, this.Q1Q2_strength);
        itr_q1q2++
      ) {
        Q[4] = this.tmp_q4;
        Q[9] = this.tmp_q9;

        // We randomly change the mask bits where QM0[i] = QM1[i] and where
        // Q[1][i] = Q[2][i]
        Q[1] = (rng() & this.mask_Q1Q2) + this.Q1_fix;
      }
    }
    return true;
  }

  calculateHash(block2) {
    let obj = createMD5Object();
    let { A0, B0, C0, D0 } = block2;
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
    return toHex(A0) + toHex(B0) + toHex(C0) + toHex(D0);
  }

  getCollision(startQ4, startQ9, NUM_BITS_Q16) {
    console.assert(startQ4 != undefined);
    console.assert(startQ9 != undefined);
    console.assert(NUM_BITS_Q16 != undefined);
    const Q = [...this.Q];
    const x = [...this.x];

    // TODO different way?
    const stub = { X: this.X, rngMask: this.rngMask };
    const rng = this.rng.bind(stub);

    let i = 0,
      itr_q16 = 0,
      itr_q1q2 = 0,
      itr_q9 = 0,
      itr_q4 = 0;
    const {
      mask_Q1Q2,
      Q1_fix,
      Q2_fix,
      Q1Q2_strength,
      QM0,
      QM1,
      QM2,
      QM3,
      tmp_q4,
      tmp_q9,
      mask_Q4,
      mask_Q9,
    } = this;

    const itr_q4_start = startQ4;
    const itr_q4_end = startQ4 + 1;
    const itr_q9_start = startQ9;
    const itr_q9_end = startQ9 + 1;
    // Start block 2 generation.
    for (itr_q16 = 0; itr_q16 < Math.pow(2, NUM_BITS_Q16); itr_q16++) {
      Q[1] = this.tmp_q1;
      Q[2] = this.tmp_q2;
      Q[4] = this.tmp_q4;
      Q[9] = this.tmp_q9;

      // Conditions by Liang-Lai says: Q[15] = (rng() & 0x80fc3ff7) +
      // 0x7d020000, Q[15] =  0111  1101  ....  ..10  00..  ....  ....  0... RNG
      // =  ....  ....  ****  **..  ..**  ****  ****  .***  0x80fc3ff7 0     =
      // *...  ..*.  ....  ...*  **..  ....  ....  *...  0x0201c008 1     = .***
      // **.*  ....  ..*.  ....  ....  ....  ....  0x7d020000
      Q[15] = (rng() & 0x00fc3ff7) + 0x7d020000;

      // Q[16] =  ^.10  ....  ....  ..01  1...  ....  ....  1...
      // RNG   =  .*..  ****  ****  **..  .***  ****  ****  .***  0x4ffc7ff7
      // 0     =  ...*  ....  ....  ..*.  ....  ....  ....  ....  0x10020000
      // 1     =  ..*.  ....  ....  ...*  *...  ....  ....  *...  0x20018008
      // Q[15] =  *.... ..... ..... .... ..... ..... ...... ....  0x80000000
      Q[16] = (rng() & 0x4ffc7ff7) + 0x20018008 + (Q[15] & 0x80000000);

      x[1] = RR(Q[2] - Q[1], 12) - F(Q[1], QM0, QM1) - QM2 - 0xe8c7b756;
      x[6] = RR(Q[7] - Q[6], 17) - F(Q[6], Q[5], Q[4]) - Q[3] - 0xa8304613;
      x[11] = RR(Q[12] - Q[11], 22) - F(Q[11], Q[10], Q[9]) - Q[8] - 0x895cd7be;

      // Q[17] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ17,25 ~ Σ17,27 not all 1
      // 0x07000000 =  0000 0111 0000 0000 0000 0000 0000 0000
      const sigma_Q17 = G(Q[16], Q[15], Q[14]) + Q[13] + x[1] + 0xf61e2562;
      if ((sigma_Q17 & 0x07000000) == 0x07000000) continue;

      // Q[16] =  ^.10  ....  ....  ..01  1...  ....  ....  1...
      // Q[17] =  ^.v.  ....  ....  ..0.  1...  ....  ....  1...
      //          1000  0000  0000  0010  1000  0000  0000  1000 0x80028008
      Q[17] = Q[16] + RL(sigma_Q17, 5);

      if ((Q[17] & 0x80028008) != (Q[16] & 0x80028008)) continue;

      // Q[18] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      Q[18] = Q[17] + RL(G(Q[17], Q[16], Q[15]) + Q[14] + x[6] + 0xc040b340, 9);

      // Q[18] =  ^.^.  ....  ....  ..1.  ....  ....  ....  ....
      if (bit(Q[18], 18) != 1) continue;

      if ((Q[18] & 0xa0000000) != (Q[17] & 0xa0000000)) continue;

      // Q[19] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
      // Extra conditions: Σ19,4 ~ Σ19,18 not all 1
      // 0x0003fff8 =  0000 0000 0000 0011 1111 1111 1111 1000
      const sigma_Q19 = G(Q[18], Q[17], Q[16]) + Q[15] + x[11] + 0x265e5a51;
      if ((sigma_Q19 & 0x0003fff8) == 0x0003fff8) continue;

      Q[19] = Q[18] + RL(sigma_Q19, 14);

      // Q[19] =  ^...  ....  ....  ..0.  ....  ....  ....  ....
      if (bit(Q[19], 18) != 0) continue;

      if (bit(Q[19], 32) != bit(Q[18], 32)) continue;

      x[10] = RR(Q[11] - Q[10], 17) - F(Q[10], Q[9], Q[8]) - Q[7] - 0xffff5bb1;
      x[15] =
        RR(Q[16] - Q[15], 22) - F(Q[15], Q[14], Q[13]) - Q[12] - 0x49b40821;

      ///////////////////////////////////////////////////////////////
      ///                      MMMM Q1/Q2                          //
      ///////////////////////////////////////////////////////////////
      // MMMM Q1/Q2 - variable bits
      for (itr_q1q2 = 0; itr_q1q2 < Math.pow(2, Q1Q2_strength); itr_q1q2++) {
        Q[4] = tmp_q4;
        Q[9] = tmp_q9;

        // We randomly change the mask bits where QM0[i] = QM1[i] and where
        // Q[1][i] = Q[2][i]
        Q[1] = (rng() & mask_Q1Q2) + Q1_fix;
        Q[2] = (Q[1] & mask_Q1Q2) + Q2_fix;

        x[0] = RR(Q[1] - QM0, 7) - F(QM0, QM1, QM2) - QM3 - 0xd76aa478;

        // Q[20] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Extra conditions: Σ20,30 ~ Σ20,32 not all 0
        // 0xe0000000 =  1110 0000 0000 0000 0000 0000 0000 0000
        const sigma_Q20 = G(Q[19], Q[18], Q[17]) + Q[16] + x[0] + 0xe9b6c7aa;
        if ((sigma_Q20 & 0xe0000000) == 0) continue;

        Q[20] = Q[19] + RL(sigma_Q20, 20);

        // Q[20] =  ^...  ....  ....  ..v.  ....  ....  ....  ....
        if (bit(Q[20], 32) != bit(Q[19], 32)) continue;

        // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        x[5] = RR(Q[6] - Q[5], 12) - F(Q[5], Q[4], Q[3]) - Q[2] - 0x4787c62a;

        Q[21] =
          Q[20] + RL(G(Q[20], Q[19], Q[18]) + Q[17] + x[5] + 0xd62f105d, 5);

        // Q[21] =  ^...  ....  ....  ..^.  ....  ....  ....  ....
        //          1000  0000  0000  0010  0000  0000  0000  0000 = 0x80020000
        if ((Q[21] & 0x80020000) != (Q[20] & 0x80020000)) continue;

        // Q[21] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        Q[22] =
          Q[21] + RL(G(Q[21], Q[20], Q[19]) + Q[18] + x[10] + 0x2441453, 9);

        // Q[22] =  ^...  ....  ....  ....  ....  ....  ....  ....
        if (bit(Q[22], 32) != bit(Q[21], 32)) continue;

        // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Extra conditions: Σ23,18 = 0
        const sigma_Q23 = G(Q[22], Q[21], Q[20]) + Q[19] + x[15] + 0xd8a1e681;
        if (bit(sigma_Q23, 18) != 0) continue;

        Q[23] = Q[22] + RL(sigma_Q23, 14);

        // Q[23] =  0...  ....  ....  ....  ....  ....  ....  ....
        if (bit(Q[23], 32) != 0) continue;

        // Q[23] ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        x[4] = RR(Q[5] - Q[4], 7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0faf;

        Q[24] =
          Q[23] + RL(G(Q[23], Q[22], Q[21]) + Q[20] + x[4] + 0xe7d3fbc8, 20);

        // Q[24] =  1...  ....  ....  ....  ....  ....  ....  ....
        if (bit(Q[24], 32) != 1) continue;

        x[2] = RR(Q[3] - Q[2], 17) - F(Q[2], Q[1], QM0) - QM1 - 0x242070db;
        x[13] =
          RR(Q[14] - Q[13], 12) - F(Q[13], Q[12], Q[11]) - Q[10] - 0xfd987193;
        x[14] =
          RR(Q[15] - Q[14], 17) - F(Q[14], Q[13], Q[12]) - Q[11] - 0xa679438e;

        ///////////////////////////////////////////////////////////////
        ///                         MMMM Q4                          //
        ///////////////////////////////////////////////////////////////
        // MMMM Q4 - 6 bits
        for (itr_q4 = itr_q4_start; itr_q4 < itr_q4_end; itr_q4++) {
          Q[4] = tmp_q4 ^ mask_Q4[itr_q4];

          x[4] = RR(Q[5] - Q[4], 7) - F(Q[4], Q[3], Q[2]) - Q[1] - 0xf57c0faf;

          Q[24] =
            Q[23] + RL(G(Q[23], Q[22], Q[21]) + Q[20] + x[4] + 0xe7d3fbc8, 20);

          // Q[24] =  1...  ....  ....  ....  ....  ....  ....  ....
          if (bit(Q[24], 32) != 1) continue;

          x[3] = RR(Q[4] - Q[3], 22) - F(Q[3], Q[2], Q[1]) - QM0 - 0xc1bdceee;
          x[7] = RR(Q[8] - Q[7], 22) - F(Q[7], Q[6], Q[5]) - Q[4] - 0xfd469501;

          ///////////////////////////////////////////////////////////////
          ///                       Tunnel Q9                          //
          ///////////////////////////////////////////////////////////////
          // Tunnel Q9 - 8 bits
          for (itr_q9 = itr_q9_start; itr_q9 < itr_q9_end; itr_q9++) {
            Q[9] = tmp_q9 ^ mask_Q9[itr_q9];

            x[8] = RR(Q[9] - Q[8], 7) - F(Q[8], Q[7], Q[6]) - Q[5] - 0x698098d8;
            x[9] =
              RR(Q[10] - Q[9], 12) - F(Q[9], Q[8], Q[7]) - Q[6] - 0x8b44f7af;
            x[12] =
              RR(Q[13] - Q[12], 7) - F(Q[12], Q[11], Q[10]) - Q[9] - 0x6b901122;

            Q[25] =
              Q[24] + RL(G(Q[24], Q[23], Q[22]) + Q[21] + x[9] + 0x21e1cde6, 5);
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
              Q[29] + RL(G(Q[29], Q[28], Q[27]) + Q[26] + x[2] + 0xfcefa3f8, 9);
            Q[31] =
              Q[30] +
              RL(G(Q[30], Q[29], Q[28]) + Q[27] + x[7] + 0x676f02d9, 14);
            Q[32] =
              Q[31] +
              RL(G(Q[31], Q[30], Q[29]) + Q[28] + x[12] + 0x8d2a4c8a, 20);
            Q[33] =
              Q[32] + RL(H(Q[32], Q[31], Q[30]) + Q[29] + x[5] + 0xfffa3942, 4);
            Q[34] =
              Q[33] +
              RL(H(Q[33], Q[32], Q[31]) + Q[30] + x[8] + 0x8771f681, 11);

            // Extra conditions: Σ35,16 = 1
            const sigma_Q35 =
              H(Q[34], Q[33], Q[32]) + Q[31] + x[11] + 0x6d9d6122;
            if (bit(sigma_Q35, 16) != 1) continue;

            Q[35] = Q[34] + RL(sigma_Q35, 16);

            Q[36] =
              Q[35] +
              RL(H(Q[35], Q[34], Q[33]) + Q[32] + x[14] + 0xfde5380c, 23);
            Q[37] =
              Q[36] + RL(H(Q[36], Q[35], Q[34]) + Q[33] + x[1] + 0xa4beea44, 4);
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
              Q[44] + RL(H(Q[44], Q[43], Q[42]) + Q[41] + x[9] + 0xd9d4d039, 4);
            Q[46] =
              Q[45] +
              RL(H(Q[45], Q[44], Q[43]) + Q[42] + x[12] + 0xe6db99e5, 11);
            Q[47] =
              Q[46] +
              RL(H(Q[46], Q[45], Q[44]) + Q[43] + x[15] + 0x1fa27cf8, 16);
            Q[48] =
              Q[47] +
              RL(H(Q[47], Q[46], Q[45]) + Q[44] + x[2] + 0xc4ac5665, 23);

            // Last sufficient conditions
            if (bit(Q[48], 32) != bit(Q[46], 32)) continue;

            Q[49] =
              Q[48] +
              RL(fI(Q[48], Q[47], Q[46]) + Q[45] + x[0] + 0xf4292244, 6);

            if (bit(Q[49], 32) != bit(Q[47], 32)) continue;

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

            // Extra conditions: Σ62,16 ~ Σ62,22 not all 0
            // 0x003f8000 =  0000 0000 0011 1111 1000 0000 0000 0000
            const sigma_Q62 =
              fI(Q[61], Q[60], Q[59]) + Q[58] + x[11] + 0xbd3af235;
            if ((sigma_Q62 & 0x003f8000) == 0) continue;

            Q[62] = Q[61] + RL(sigma_Q62, 10);

            // Not necessary
            // if (bit(Q[62], 26) != 1) continue;

            if (bit(Q[62], 32) != bit(Q[60], 32)) continue;

            Q[63] =
              Q[62] +
              RL(fI(Q[62], Q[61], Q[60]) + Q[59] + x[2] + 0x2ad7d2bb, 15);

            // Not necessary
            // if (bit(Q[63], 26) != 1) continue;

            if (bit(Q[63], 32) != bit(Q[61], 32)) continue;

            Q[64] =
              Q[63] +
              RL(fI(Q[63], Q[62], Q[61]) + Q[60] + x[9] + 0xeb86d391, 21);

            // Condition not necessary (Sasaki), try to remove
            // if (bit(Q[64], 26) != 1) continue;

            // Block 2 is now completed. We verify if the differential path is
            // reached.

            // Message 1 intermediate hash
            const AA0 = this.A0 + Q[61];
            const BB0 = this.B0 + Q[64];
            const CC0 = this.C0 + Q[63];
            const DD0 = this.D0 + Q[62];

            let obj = createMD5Object();
            let { Hx } = obj;
            // Message 2 intermediate hash computation
            for (i = 0; i < 16; i++) Hx[i] = x[i];

            Hx[4] = x[4] - 0x80000000;
            Hx[11] = x[11] - 0x00008000;
            Hx[14] = x[14] - 0x80000000;

            obj.a = this.A1 >>> 0;
            obj.b = this.B1 >>> 0;
            obj.c = this.C1 >>> 0;
            obj.d = this.D1 >>> 0;

            obj = HMD5Tr(obj);

            const AA1 = (this.A1 + obj.a) >>> 0;
            const BB1 = (this.B1 + obj.b) >>> 0;
            const CC1 = (this.C1 + obj.c) >>> 0;
            const DD1 = (this.D1 + obj.d) >>> 0;

            if (
              (AA1 - AA0) >>> 0 != 0 ||
              (BB1 - BB0) >>> 0 != 0 ||
              (CC1 - CC0) >>> 0 != 0 ||
              (DD1 - DD0) >>> 0 != 0
            ) {
              continue;
            }

            // We have now found a collision!!

            // I save the last intermediate hash for final hash computation
            const A0 = AA0;
            const B0 = BB0;
            const C0 = CC0;
            const D0 = DD0;


            const v1 = [...this.v1];
            const v2 = [...this.v2];

            // I save both second blocks
            for (i = 0; i < 16; i++) {
              memcpy(v1, 64 + i * 4, x, i, 4);
              memcpy(v2, 64 + i * 4, Hx, i, 4);
            }
            // console.log(v1.map(x => x.toString(16)));
            // console.log(v2.map(x => x.toString(16)));
            // console.log("itr_Q4: ", itr_q4);
            // console.log("itr_Q9: ", itr_q9);
            // if (v2[82] == 0xf6){
            //   console.assert(v2[83] == 0xa4);
            // }
            return { v1, v2, A0, B0, C0, D0 };
          } // End of Tunnel Q9
        } // End of MMMM Q4
      } // End of MMMM Q1/12
    } // End of MMMM Q16
    return undefined; // Collision not found
  }

  determineTunnelValues(x, y, output) {
    let id = (x + y * 256) >>> 0;
    const Q4_strength = 6;
    const startQ4 = id & ((1 << Q4_strength) - 1);
    id = id >>> Q4_strength;
    const Q9_strength = 8;
    const startQ9 = id & ((1 << Q9_strength) - 1);
    id = id >>> Q9_strength;
    const values = {
      startQ4,
      startQ9,
    };
    return values;
  }
}
