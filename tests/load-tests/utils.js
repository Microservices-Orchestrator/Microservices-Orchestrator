/**
 * Rastgele bir IPv4 adresi üretir.
 * @returns {string} Örn: "123.45.67.89"
 */
export function generateRandomIp() {
  return `${Math.floor(Math.random() * 255) + 1}.${Math.floor(
    Math.random() * 255
  )}.${Math.floor(Math.random() * 255)}.${Math.floor(Math.random() * 255)}`;
}

/**
 * Önceden tanımlanmış bir listeden rastgele bir tehdit türü seçer.
 * @returns {string} Örn: "SQL_Injection"
 */
export function generateRandomThreatType() {
  const threatTypes = ['SQL_Injection', 'XSS_Attack', 'Port_Scan', 'Malware_Infection', 'Brute_Force_Attempt', 'Phishing_Link'];
  return threatTypes[Math.floor(Math.random() * threatTypes.length)];
}

/**
 * Rastgele 8 karakterli bir şifre üretir.
 * @returns {string} Örn: "a1b2c3d4"
 */
export function generateRandomPassword() {
    return Math.random().toString(36).slice(-8);
}