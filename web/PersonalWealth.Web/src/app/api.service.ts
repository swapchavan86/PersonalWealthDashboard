import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
export interface Dashboard { cash:number; investments:number; otherAssets:number; liabilities:number; netWorth:number; }
export interface InvestmentHolding { id:string; investmentAccountId:string; securityId:string; quantity:number; costBasis:number; }
@Injectable({providedIn:'root'}) export class ApiService { private http=inject(HttpClient); dashboard():Observable<Dashboard>{return this.http.get<Dashboard>('/api/v1/wealth/dashboard');} holdings():Observable<InvestmentHolding[]>{return this.http.get<InvestmentHolding[]>('/api/v1/investments/holdings');} }
