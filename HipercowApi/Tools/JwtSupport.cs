// Copyright (c) Imperial College London. All rights reserved.

namespace HipercowApi.Tools
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using Jose;

    /// <summary>
    /// Support for creating and encrypting the JWT.
    /// </summary>
    public class JwtSupport
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JwtSupport"/> class.
        /// </summary>
        public JwtSupport()
        {
            SigningKey = GenerateRandomKey(32);    // 256-bit for HMAC-SHA256
            EncryptionKey = GenerateRandomKey(32); // 256-bit for AES-256
        }

        /// <summary>
        /// Gets sets the Signing Key.
        /// </summary>
        public byte[] SigningKey { get; private set; }

        /// <summary>
        /// Gets the Encryption Key.
        /// </summary>
        public byte[] EncryptionKey { get; private set; }

        /// <summary>
        /// Decrypt an encrypted JWT.
        /// </summary>
        /// <param name="token">The encrypted token.</param>
        /// <returns>The dictionary of unencrypted keys and values.</returns>
        public Dictionary<string, object> DecryptToken(string token)
        {
            string signedJwt = JWT.Decode(
                    token,
                    EncryptionKey,
                    JweAlgorithm.A256KW,
                    JweEncryption.A256CBC_HS512);

            var payload = JWT.Decode<Dictionary<string, object>>(
                signedJwt,
                SigningKey,
                JwsAlgorithm.HS256);

            return payload;
        }

        /// <summary>
        /// Generate the encrypted JWT.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="wpia_hn_access">Whether the user has access to the wpia-hn cluster.</param>
        /// <param name="wpia_hn_admin">Whether the user is an admin on the wpia-hn cluster.</param>
        /// <returns>The string of the token.</returns>
        public string GenerateEncryptedToken(
            string username,
            string password,
            bool wpia_hn_access,
            bool wpia_hn_admin)
        {
            var payload = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, username },
            { ClaimTypes.Name, username },
            { ClaimTypes.NameIdentifier, username },
            { "password", password },
            { "wpia_hn_access", wpia_hn_access },
            { "wpia_hn_admin", wpia_hn_admin },
            { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() },
            { JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
            { JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(60 * 24).ToUnixTimeSeconds() },
            { JwtRegisteredClaimNames.Iss, "Hipercow API" },
            { JwtRegisteredClaimNames.Aud, "Hipercow Users" },
        };

            string signedJwt = JWT.Encode(payload, SigningKey, JwsAlgorithm.HS256);

            string encryptedJwt = JWT.EncodeBytes(
                Encoding.UTF8.GetBytes(signedJwt),
                EncryptionKey,
                JweAlgorithm.A256KW,
                JweEncryption.A256CBC_HS512,
                extraHeaders: new Dictionary<string, object> { { "cty", "JWT" } });

            return encryptedJwt;
        }

        private static byte[] GenerateRandomKey(int size)
        {
            var key = new byte[size];
            RandomNumberGenerator.Fill(key);
            return key;
        }
    }
}
