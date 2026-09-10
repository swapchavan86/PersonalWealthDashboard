import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
interface Dashboard { cash:number; investments:number; otherAssets:number; liabilities:number; netWorth:number; }
@Component({selector:'app-root',standalone:true,imports:[CommonModule],template:`<main><h1>Personal Wealth</h1><p>Auditable financial overview</p><section><article>Cash <strong>{{d?.cash | number:'1.2-2'}}</strong></article><article>Investments <strong>{{d?.investments | number:'1.2-2'}}</strong></article><article>Other assets <strong>{{d?.otherAssets | number:'1.2-2'}}</strong></article><article>Liabilities <strong>{{d?.liabilities | number:'1.2-2'}}</strong></article></section><div>Net worth <strong>{{d?.netWorth | number:'1.2-2'}}</strong></div></main>`})
class AppComponent { private http=inject(HttpClient); d?:Dashboard; constructor(){this.http.get<Dashboard>('/api/v1/wealth/dashboard').subscribe(x=>this.d=x);} }
bootstrapApplication(AppComponent,{providers:[provideHttpClient()]});
