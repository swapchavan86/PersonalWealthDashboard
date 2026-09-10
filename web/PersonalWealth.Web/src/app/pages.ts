import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from './api.service';
@Component({standalone:true,imports:[CommonModule],template:`<h2>Wealth dashboard</h2><p>Net worth</p><strong>{{d?.netWorth|number:'1.2-2'}}</strong><p>Cash {{d?.cash|number:'1.2-2'}} · Investments {{d?.investments|number:'1.2-2'}} · Assets {{d?.otherAssets|number:'1.2-2'}} · Liabilities {{d?.liabilities|number:'1.2-2'}}</p>`}) export class DashboardPage { private api=inject(ApiService); d?:ReturnType<ApiService['dashboard']> extends infer _ ? any : never; constructor(){this.api.dashboard().subscribe(x=>this.d=x);} }
@Component({standalone:true,template:`<h2>Banking</h2><p>Accounts, transactions, categorization and reconciliation are available through the versioned API.</p>`}) export class BankingPage {}
@Component({standalone:true,template:`<h2>Expenses</h2><p>Expense categories, recurring rules, reporting and transaction linking are available through the application layer.</p>`}) export class ExpensesPage {}
@Component({standalone:true,imports:[CommonModule],template:`<h2>Investments</h2><p>Holdings are sourced from the deterministic investment ledger.</p><ul><li *ngFor="let h of holdings">{{h.securityId}} — {{h.quantity}} units — {{h.costBasis|number:'1.2-2'}}</li></ul>`}) export class InvestmentsPage { private api=inject(ApiService); holdings:any[]=[]; constructor(){this.api.holdings().subscribe(x=>this.holdings=x);} }
@Component({standalone:true,template:`<h2>Assets</h2><p>Physical and financial assets with valuation history.</p>`}) export class AssetsPage {}
@Component({standalone:true,template:`<h2>Liabilities</h2><p>Loans, repayments, interest and outstanding balances.</p>`}) export class LiabilitiesPage {}
@Component({standalone:true,template:`<h2>Imports</h2><p>Deterministic document and investment-import validation workflows.</p>`}) export class ImportsPage {}
