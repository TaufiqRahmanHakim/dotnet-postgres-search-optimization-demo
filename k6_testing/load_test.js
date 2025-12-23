import http from 'k6/http';
import { check, sleep } from 'k6';
// 1. Import library reporter (langsung dari internet, tidak perlu install npm)
//import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

export const options = {
  vus: 500,
  duration: '100s',
  insecureSkipTLSVerify: true, 
  thresholds: {
    'http_req_failed': ['rate<0.05'], 
    
    // Response time: 90% request harus di bawah 500ms
    'http_req_duration': ['p(90)<500'],

    // Custom Checks: LULUS jika > 80% check berhasil
    'checks': ['rate>0.80'],
  }
};

export default function () {
  const res = http.get('https://localhost:7289/api/Customer?search=Dimaz%20Laksmiwati&page=1&pageSize=10');
  //const res = http.get('https://localhost:7289/api/Customer/fast-paging?lastSeenId=0&limit=20');

  check(res, {
    'status is 200': (r) => r.status === 200,
    'response time < 500ms': (r) => r.timings.duration < 500,
    'response time < 200ms': (r) => r.timings.duration < 200,
    'response time < 50ms': (r) => r.timings.duration < 50,
    'response time < 10ms': (r) => r.timings.duration < 10,
    'response time < 5ms': (r) => r.timings.duration < 5,
  });

  sleep(1);
}

// 2. Fungsi ini akan jalan otomatis setelah test selesai untuk bikin file HTML
// export function handleSummary(data) {
//   return {
//     "report_benchmark.html": htmlReport(data),
//   };
// }


//copy this to terminal:
//k6 run --out web-dashboard load_test.js
//k6 run --out web-dashboard=export=laporan_baru.html load_test.js