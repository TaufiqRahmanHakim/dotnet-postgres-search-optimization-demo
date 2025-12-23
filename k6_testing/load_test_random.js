import http from 'k6/http';
import { check, sleep } from 'k6';
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

// Kita siapkan "Kolam Data" untuk simulasi search user
const searchTerms = [
  "Budi", "Siti", "Dimaz", "Andi", "Lestari", // Nama-nama umum
  "Jakarta", "Bandung", "Surabaya",           // Kota                          
  ""                                          // Kosong (artinya user tidak search)
];

export const options = {
  vus: 500,
  duration: '100s',
  insecureSkipTLSVerify: true,
  thresholds: {
    'checks': ['rate>0.80'], 
    'http_req_duration': ['p(95)<1000'], // Sedikit dilonggarkan karena data dinamis
  }
};

// Helper: Fungsi acak angka min s/d max
function randomInt(min, max) {
  return Math.floor(Math.random() * (max - min + 1) + min);
}

// Helper: Ambil item acak dari array
function randomItem(arr) {
  return arr[Math.floor(Math.random() * arr.length)];
}

export default function () {
  // --- SKENARIO DINAMIS ---
  
  // Kita lempar dadu (0.0 - 1.0) untuk menentukan user ini mau ngapain
  const scenarioDice = Math.random();

  let res;

  const searchTerm = randomItem(searchTerms);
    const page = randomInt(1, 5);       // User melihat halaman 1 sampai 5
    const pageSize = randomItem([10, 20, 50]); // User ganti-ganti row per page

    // Tips: Gunakan 'encodeURIComponent' biar spasi aman
    const url = `https://localhost:7289/api/Customer?search=${encodeURIComponent(searchTerm)}&page=${page}&pageSize=${pageSize}`;

    // PENTING: Tambahkan 'tags' { name: ... }
    // Supaya di report k6 tidak muncul ribuan URL berbeda, tapi digabung jadi satu grup "GetCustomers"
    res = http.get(url, { tags: { name: 'GetCustomers' } });

  // --- CHECK ---
  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
    'response time < 200ms': (r) => r.timings.duration < 200,
    'response time < 50ms': (r) => r.timings.duration < 50,
    'response time < 10ms': (r) => r.timings.duration < 10,
    'response time < 5ms': (r) => r.timings.duration < 5,
    'response time < 2ms': (r) => r.timings.duration < 2,
  });

  // Random sleep 0.5s - 1.5s biar lebih natural kayak manusia
  sleep(Math.random() * 1 + 0.5);
}
export function handleSummary(data) {
  return {
    "report_benchmark_random.html": htmlReport(data),
  };
}
//k6 run --out web-dashboard load_test_random.js