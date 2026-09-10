import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface Dashboard { cash:number; investments:number; otherAssets:number; liabilities:number; netWorth:number; }
@Component({selector:'app-root',standalone:true,template:`<main><header><h1>Personal Wealth</h1><p>Auditable financial overview</p></header><section class="grid"><article><span>Cash</span><strong>{{d?.cash | number:'1.2-2'}}</strong></article><article><span>Investments</span><strong>{{d?.investments | number:'1.2-2'}}</strong></article><article><span>Other assets</span><strong>{{d?.otherAssets | number:'1.2-2'}}</strong></article><article><span>Liabilities</span><strong>{{d?.liabilities | number:'1.2-2'}}</strong></article></section><section class="net"><span>Net worth</span><strong>{{d?.netWorth | number:'1.2-2'}}</strong></section></main>`,styles:[`main{max-width:1100px;margin:40px auto;padding:0 24px;font-family:system-ui}header{margin-bottom:28px}.grid{display:grid;grid-template-columns:repeat(4,1fr);gap:16px}.grid article,.net{border:1px solid #ddd;border-radius:12px;padding:20px;background:#fff}.grid span,.net span{display:block;color:#666}.grid strong,.net strong{display:block;font-size:24px;margin-top:8px}.net{margin-top:16px} @media(max-width:800px){.grid{grid-template-columns:1fr 1fr}}`],imports:[]})
class AppComponent { private http=inject(HttpClient); d?:Dashboard; constructor(){this.http.get<Dashboard>('/api/v1/wealth/dashboard').subscribe(x=>this.d=x);} }
bootstrapApplication(AppComponent,{providers:[provideHttpClient()]});
