import {Component,inject} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ApiService,Dashboard,InvestmentHolding,WealthImportResult} from './api.service';
import * as XLSX from 'xlsx';

@Component({standalone:true,imports:[CommonModule],template:`<h2>Wealth dashboard</h2><p>Net worth</p><strong>{{d?.netWorth|number:'1.2-2'}}</strong><p>Cash {{d?.cash|number:'1.2-2'}} · Investments {{d?.investments|number:'1.2-2'}} · Assets {{d?.otherAssets|number:'1.2-2'}} · Liabilities {{d?.liabilities|number:'1.2-2'}}</p>`}) export class DashboardPage {private api=inject(ApiService);d?:Dashboard;constructor(){this.api.dashboard().subscribe(x=>this.d=x);}}
@Component({standalone:true,template:`<h2>Banking</h2><p>Accounts, transactions, categorization and reconciliation are available through the versioned API.</p>`}) export class BankingPage {}
@Component({standalone:true,template:`<h2>Expenses</h2><p>Expense categories, recurring rules, reporting and transaction linking are available through the application layer.</p>`}) export class ExpensesPage {}
@Component({standalone:true,imports:[CommonModule],template:`<h2>Investments</h2><p>Holdings are sourced from the deterministic investment ledger.</p><ul><li *ngFor="let h of holdings">{{h.securityId}} — {{h.quantity}} units — {{h.costBasis|number:'1.2-2'}}</li></ul>`}) export class InvestmentsPage {private api=inject(ApiService);holdings:InvestmentHolding[]=[];constructor(){this.api.holdings().subscribe(x=>this.holdings=x);}}
@Component({standalone:true,template:`<h2>Assets</h2><p>Physical and financial assets with valuation history.</p>`}) export class AssetsPage {}
@Component({standalone:true,template:`<h2>Liabilities</h2><p>Loans, repayments, interest and outstanding balances.</p>`}) export class LiabilitiesPage {}

@Component({standalone:true,imports:[CommonModule],template:`
<h2>Imports</h2>
<p>Use the Excel workbook template. Each financial data type has its own sheet with only the columns relevant to that type.</p>
<p><strong>Validation is non-destructive:</strong> invalid cells are reported with the sheet name and Excel cell address. No database changes are made when validation fails.</p>
<button (click)="downloadTemplate()">Download Excel template</button>
<input type="file" accept=".xlsx,.csv" (change)="select($event)">
<button [disabled]="!file||busy" (click)="upload()">{{busy?'Validating / importing…':'Upload and import'}}</button>
<p>{{message}}</p>
<section *ngIf="result?.errors?.length" style="margin-top:1rem">
  <h3>Fix these cells and upload again</h3>
  <p>{{result?.errors?.length}} validation error(s) found.</p>
  <ol><li *ngFor="let error of result?.errors">{{error}}</li></ol>
</section>
<pre *ngIf="result && !result.errors?.length">{{result|json}}</pre>
`,styles:[`section{max-width:900px}.errors{font-family:monospace}`]})
export class ImportsPage {
  private api=inject(ApiService); file?:File; busy=false; message=''; result?:WealthImportResult;
  select(e:Event){const i=e.target as HTMLInputElement;this.file=i.files?.[0];this.result=undefined;this.message=this.file?`Selected ${this.file.name}`:'';}
  upload(){if(!this.file)return;this.busy=true;this.result=undefined;this.api.importWealth(this.file).subscribe({next:r=>{this.result=r;this.busy=false;this.message=r.success?`Imported ${r.rowsImported} records.`:`Import blocked. Correct the ${r.errors.length} reported cell/sheet issue(s) and upload again.`;},error:e=>{this.busy=false;this.message=e?.error?.errors?.join(' ')||'Import failed. No data was imported.';}});}
  downloadTemplate(){
    const wb=XLSX.utils.book_new();
    const sheets:Record<string,unknown[][]>={
      'Bank Accounts':[['ExternalId','Institution','AccountNumber','AccountType','Currency'],['bank-hdfc','HDFC Bank','HDFC-001','Savings','INR']],
      'Bank Transactions':[['ExternalId','Date','AccountNumber','Currency','Description','Direction','Amount','Category'],['txn-salary-2026-08-01','2026-08-01','HDFC-001','INR','Salary','Credit',150000,'Income'],['txn-rent-2026-08-05','2026-08-05','HDFC-001','INR','Rent','Debit',35000,'Housing']],
      'Expenses':[['ExternalId','Date','AccountNumber','Currency','Description','Amount','Category'],['exp-rent-2026-08-05','2026-08-05','HDFC-001','INR','Monthly rent',35000,'Housing']],
      'Investment Accounts':[['ExternalId','Institution','AccountNumber','AccountType','Currency'],['inv-zerodha','Zerodha','Z-001','Brokerage','INR']],
      'Securities':[['ExternalId','SecuritySymbol','SecurityName','SecurityType','Currency'],['sec-reliance','RELIANCE','Reliance Industries','Equity','INR']],
      'Investment Transactions':[['ExternalId','Date','AccountNumber','Currency','Description','Direction','SecuritySymbol','Quantity','UnitPrice','Fees'],['inv-buy-reliance','2026-08-10','Z-001','INR','Purchase','Buy','RELIANCE',20,2800,20]],
      'Assets':[['ExternalId','Date','AssetName','AssetType','Currency','AcquisitionValue'],['asset-home','2024-01-01','Home','Property','INR',8000000]],
      'Asset Valuations':[['ExternalId','Date','AssetName','ValuationValue'],['asset-home-val','2026-08-31','Home',8500000]],
      'Liabilities':[['ExternalId','Date','LiabilityName','LiabilityType','Currency','Principal','InterestRate','MaturityDate'],['loan-home','2024-01-01','Home Loan','Mortgage','INR',5000000,8.5,'2029-01-01']],
      'Liability Repayments':[['ExternalId','Date','LiabilityName','Currency','Amount','PrincipalAmount','InterestAmount'],['loan-home-repay-2026-08','2026-08-15','Home Loan','INR',50000,40000,10000]],
      'Instructions':[['Personal Wealth Import Template'],['Do not rename the financial sheets or their columns.'],['Enter one record per row. Keep dates as YYYY-MM-DD and numeric fields as numbers.'],['If a value is invalid, the application reports the sheet and Excel cell address and imports nothing from the workbook.'],['Upload this workbook from the Imports page after correcting all reported issues.']]
    };
    Object.entries(sheets).forEach(([name,data])=>{const ws=XLSX.utils.aoa_to_sheet(data);ws['!freeze']={xSplit:0,ySplit:1};ws['!autofilter']={ref:XLSX.utils.encode_range({s:{r:0,c:0},e:{r:0,c:data[0].length-1}})};XLSX.utils.book_append_sheet(wb,ws,name);});
    XLSX.writeFile(wb,'PersonalWealth_Import_Template.xlsx');
  }
}
