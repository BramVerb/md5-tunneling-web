# md5-tunneling
A web version of the C implementation of the Tunneling Method by V. Klima to speed up collision search for MD5.

It uses webgl to apply tunnels in parralel and find collisions faster than the original implementation.
Results do very depending on GPU setup. I was able to generate a collisions in an average of 1.6s on a notebook GPU (Quadro M1200).
The C version on the same laptop took on average about 11 seconds per collision.
NOTE: You might experience some dropped frames, because it does use your GPU.

## Demo
A demo can be found here:
[md5-tunneling-web/](https://bramverb.github.io/md5-tunneling-web/)

## About
Based on Multi-Message Modifications Method and Tunnels, the program searches for two 128-bytes messages with same [MD5](https://en.wikipedia.org/wiki/MD5) hash.

The idea of Tunnels to speed up MD5 collisions search is described in: *Vlastimil Klima: Tunnels in Hash Functions: MD5 Collisions Within a Minute, sent to IACR eprint, 18 March, 2006* [PDF](http://eprint.iacr.org/2006/105.pdf)

Other related papers and attack examples can be found [here](http://cryptography.hyperlink.cz/MD5_collisions.html).

## Usage
Start a webserver in the `web/` folder.
Go to that url.
For example using python3's `http.server`

```
cd web/
python3 -m http.server
```

You can now go to: `localhost:8000`

## Original Functionalities (have not been ported to this web implementation)
At compile time, the user can choose to
* Enable/disable any implemented tunnel
* Write to disk the two colliding messages as binaries
* Write to disk a summary of the collision found (message bytes, timings and common hash)
* Print the colliding hash in summary
* Print the colliding hash in standard output

A [Linear Congruential Generator](https://en.wikipedia.org/wiki/Linear_congruential_generator) is implemented as a pseudo-random number generator. The user can set a seed that produces the same collision across different OS and architectures.

The attack is independent from the Init Vector (IV) used by MD5. The user can set the IV he wants and search for a collision for that particular IV.

You can give as input to the program:
* 1 hex number to specify the seed
```
md5-tunneling 0x69423840
```
* 4 hex numbers to specify the custom IV for MD5
```
md5-tunneling 0xF0E1D2C3 0xB4A59687 0x78695A4B 0x3C2D1E0F
```
* 5 hex numbers to specify the seed and custom IV
```
md5-tunneling 0x69423840 0xF0E1D2C3 0xB4A59687 0x78695A4B 0x3C2D1E0F
```
