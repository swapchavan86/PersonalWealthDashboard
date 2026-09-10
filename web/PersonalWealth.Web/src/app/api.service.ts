import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Dashboard {
  cash: number;
  investments: number;
  otherAssets: number;
  liabilities: number;
  netWorth: number;
}

export interface InvestmentHolding {
  id: string;
  investmentAccountId: string;
  securityId: string;
  quantity: number;
  costBasis: number;
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly http = inject(HttpClient);

  private readonly developmentHeaders = new HttpHeaders({
    'X-Tenant-Id': '11111111-1111-1111-1111-111111111111',
    'X-Dev-User': 'local-dev-user'
  });

  dashboard(): Observable<Dashboard> {
    return this.http.get<Dashboard>('/api/v1/wealth/dashboard', { headers: this.developmentHeaders });
  }

  holdings(): Observable<InvestmentHolding[]> {
    return this.http.get<InvestmentHolding[]>('/api/v1/investments/holdings', { headers: this.developmentHeaders });
  }
}
