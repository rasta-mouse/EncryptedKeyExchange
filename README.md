# Encrypted Key Exchange

My attempt at implementing Encrypted Key Exchange in .NET between a web app and a client.
I know nothing abut crypto or how this is *supposed* to work.

High level steps:
- Both Server and Client generate RSA public/private key pairs on startup.
- Client sends its public key to the server.  Server responds with its own public key.
- Client generates a random challenge, encrypts it with the server's public key and sends it to the server.  Server decrypts and stores the challenge.  Server generates a new session key, encrypts it with the client's public key and sends it back to the client.
- Client decrypts and stores the session key.
- Client requests its orignal challenge back. Server encrypts the challenge with the new session key.
- Client decrypts challenge with session key and verifies it matches what it originally sent.


## Usage
### Server

```text
rasta@MBP Server % dotnet run
Building...
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:8080
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/rasta/RiderProjects/EncryptedKeyExchange/Server

Public Key from Client (9c261a67-f347-4960-967a-19322757dfa3): <RSAKeyValue><Modulus>4+7Fbe0bdikHD3dBYeNATN5nRWq78FCL4xwjfIMhhqqq9HOu/hkKvWCrEwY4pwqdZ+JKYnu0GG2IH3qYVVaexmoVGPp8LAKvp+IuLM7FZ6hkT/6Ai+NEFqhFVmuMqdfHUfLfBX6HzvSvNV3yDyzmYvZDBsf9ryFdjj1h0k61avBscS4mJYHDuq+qwn7pELW/x/mcGEFhwBsqeCJSig+CEoAvWsRy+TYFVpIvIsZGAEOANbMR4FQDVjHu3BVOkvhBOXytYVo1r+PpT4g6VcVYMFJrx4EmvoflRkJCKF5zSm/af8uF8nvY1zsXYvF0RLY7MsLrJpG2RT4ARnmlRg1u6Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>
Challenge from Client (9c261a67-f347-4960-967a-19322757dfa3): GslZuoTAT4SzUa2khcrq7u2JgQUa2zYoe6gvwzLC3O0uJm/0goJ0e82wr84WVjXhymTm1ImRJ/ky1Zi3qQJAiGrvD7cDSiY9nBanHm0LjuKDeSdSRNdSnIx8GXcP7k4RjiQzN42vXJjEzuG2KrYKjbq8G2Gbo11jKAxzYq3vn7c=
```

### Client

```text
rasta@MBP Client % dotnet run
Server Public Key: <RSAKeyValue><Modulus>vjULybnViJdnTlh0bo+VEVKPhU+PgijvP7YbJBhe9yLhSuZ0VQm73mrTy/1+iHhgVt0SadbvNsIs5BXEjKXLyCznrC2lz4vdZyl5DNItQaKHBY+LDrsh8bSJ7nujIusGzx3RgWKFRp6VNBfMcgwwYW+SZDwg6U3B/Swts5ME0NjgN6XyrDIxLY6/BDz9tEWMMbjc6j3G4vPDhijY6zc0KcDcl1AKrEis3J/ANFKk4F1d6JftNDBT+mfWqxD3NBeZKo2m2v4stFMAldgwp2NT/7gkIxQY8ce+2L3tA8vgS5qKRgAq5foiUMGDvg9W6Tgy0GmzZJHyaZVwrK++iJTCrQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>
Generated Challenge: GslZuoTAT4SzUa2khcrq7u2JgQUa2zYoe6gvwzLC3O0uJm/0goJ0e82wr84WVjXhymTm1ImRJ/ky1Zi3qQJAiGrvD7cDSiY9nBanHm0LjuKDeSdSRNdSnIx8GXcP7k4RjiQzN42vXJjEzuG2KrYKjbq8G2Gbo11jKAxzYq3vn7c=
Session Key: WcDjtnr31uYE7wpwt2LNfRsdnn7zlI7hnW7UTIDIH4E=
Challenge Response: GslZuoTAT4SzUa2khcrq7u2JgQUa2zYoe6gvwzLC3O0uJm/0goJ0e82wr84WVjXhymTm1ImRJ/ky1Zi3qQJAiGrvD7cDSiY9nBanHm0LjuKDeSdSRNdSnIx8GXcP7k4RjiQzN42vXJjEzuG2KrYKjbq8G2Gbo11jKAxzYq3vn7c=

Challenges match
```