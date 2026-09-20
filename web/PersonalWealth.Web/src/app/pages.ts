import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService, Dashboard, InvestmentHolding, WealthImportResult } from './api.service';
import * as XLSX from 'xlsx';

const currency = new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR', maximumFractionDigits: 0 });

function money(value: number | undefined) {
  return value == null ? '₹0' : currency.format(value);
}

@Component({
  standalone: true,
  imports: [CommonModule],
  template: `
  <div class="page">
    <div class="page-head">
      <div>
        <h2 class="page-title">Your financial picture</h2>
        <p class="page-subtitle">A calm view of what you own, what you owe and how your wealth is moving.</p>
      </div>
    </div>

    <div class="dashboard-hero">
      <article class="card net-worth-card">
        <div class="net-worth-copy">
          <div class="net-worth-label">Current net worth</div>
          <div class="net-worth-value">{{money(d?.netWorth)}}</div>
          <div class="net-worth-note">Assets minus liabilities, based on your latest available data.</div>
        </div>
        <div class="sparkline" aria-hidden="true">
          <svg viewBox="0 0 760 120" preserveAspectRatio="none">
            <path class="fill" d="M0,96 C92,82 118,90 182,65 C238,43 278,63 329,54 C400,42 450,50 504,30 C562,10 609,28 655,18 C698,9 724,13 760,4 L760,120 L0,120 Z"></path>
            <path d="M0,96 C92,82 118,90 182,65 C238,43 278,63 329,54 C400,42 450,50 504,30 C562,10 609,28 655,18 C698,9 724,13 760,4"></path>
          </svg>
        </div>
      </article>

      <article class="card goal-card">
        <div class="goal-art" aria-hidden="true"></div>
        <div>
          <div class="goal-title">The bigger picture</div>
          <div class="goal-copy">Keep an eye on your mix of cash, investments and long-term assets.</div>
          <div class="goal-bar"><span></span></div>
        </div>
      </article>
    </div>

    <div class="card-grid four">
      <article class="card metric-card">
        <div class="metric-icon"><span class="icon-wallet"></span></div>
        <div class="metric-label">Cash available</div>
        <div class="metric-value">{{money(d?.cash)}}</div>
        <div class="metric-detail">Across your tracked accounts</div>
      </article>
      <article class="card metric-card">
        <div class="metric-icon"><span class="icon-chart"></span></div>
        <div class="metric-label">Investments</div>
        <div class="metric-value">{{money(d?.investments)}}</div>
        <div class="metric-detail">Market holdings and portfolio value</div>
      </article>
      <article class="card metric-card">
        <div class="metric-icon"><span class="icon-home"></span></div>
        <div class="metric-label">Other assets</div>
        <div class="metric-value">{{money(d?.otherAssets)}}</div>
        <div class="metric-detail">Property and tracked valuables</div>
      </article>
      <article class="card metric-card">
        <div class="metric-icon"><span class="icon-shield"></span></div>
        <div class="metric-label">Liabilities</div>
        <div class="metric-value">{{money(d?.liabilities)}}</div>
        <div class="metric-detail">Loans and outstanding balances</div>
      </article>
    </div>

    <div class="card-grid two" style="margin-top:16px">
      <article class="card section-card">
        <div class="section-card-head">
          <div class="section-card-title">Where your wealth sits</div>
          <div class="section-card-link">Current mix</div>
        </div>
        <div class="allocation">
          <div class="allocation-row">
            <div class="allocation-name">Investments</div>
            <div class="allocation-track"><span style="width:62%"></span></div>
            <div class="allocation-value">62%</div>
          </div>
          <div class="allocation-row">
            <div class="allocation-name">Cash</div>
            <div class="allocation-track"><span style="width:18%"></span></div>
            <div class="allocation-value">18%</div>
          </div>
          <div class="allocation-row">
            <div class="allocation-name">Assets</div>
            <div class="allocation-track"><span style="width:20%"></span></div>
            <div class="allocation-value">20%</div>
          </div>
        </div>
      </article>

      <article class="card insight">
        <div class="insight-kicker">A useful place to start</div>
        <div class="insight-title">Your dashboard works best when the data stays simple and current.</div>
        <div class="insight-copy">Add or refresh your accounts and holdings, then use this view to see the bigger pattern without digging through spreadsheets.</div>
      </article>
    </div>
  </div>
  `
})
export class DashboardPage {
  private api = inject(ApiService);
  d?: Dashboard;
  money = money;

  constructor() {
    this.api.dashboard().subscribe({
      next: x => this.d = x,
      error: () => this.d = { cash: 0, investments: 0, otherAssets: 0, liabilities: 0, netWorth: 0 }
    });
  }
}

@Component({
  standalone: true,
  template: `
  <div class="page">
    <div class="page-head">
      <div><h2 class="page-title">Banking</h2><p class="page-subtitle">Keep your everyday money easy to understand.</p></div>
    </div>
    <div class="card-grid two">
      <article class="card section-card">
        <div class="section-card-head"><div class="section-card-title">Accounts</div></div>
        <div class="empty-card"><div class="empty-icon"><span class="icon-bank"></span></div><div class="empty-title">Your account view is ready</div><div class="empty-copy">Connect the banking API to surface balances and recent activity here.</div></div>
      </article>
      <article class="card section-card">
        <div class="section-card-head"><div class="section-card-title">Recent activity</div></div>
        <div class="empty-card"><div class="empty-icon"><span class="icon-wallet"></span></div><div class="empty-title">Nothing to review yet</div><div class="empty-copy">Once transactions are available, this space will stay focused on what matters most.</div></div>
      </article>
    </div>
  </div>`
})
export class BankingPage {}

@Component({
  standalone: true,
  template: `
  <div class="page">
    <div class="page-head">
      <div><h2 class="page-title">Expenses</h2><p class="page-subtitle">See where your money is going without the clutter.</p></div>
    </div>
    <div class="card-grid two">
      <article class="card section-card">
        <div class="section-card-head"><div class="section-card-title">Spending overview</div></div>
        <div class="allocation">
          <div class="allocation-row"><div class="allocation-name">Housing</div><div class="allocation-track"><span style="width:46%"></span></div><div class="allocation-value">46%</div></div>
          <div class="allocation-row"><div class="allocation-name">Lifestyle</div><div class="allocation-track"><span style="width:28%"></span></div><div class="allocation-value">28%</div></div>
          <div class="allocation-row"><div class="allocation-name">Other</div><div class="allocation-track"><span style="width:26%"></span></div><div class="allocation-value">26%</div></div>
        </div>
      </article>
      <article class="card insight">
        <div class="insight-kicker">Keep it useful</div>
        <div class="insight-title">Good expense tracking should help you notice patterns, not create more work.</div>
        <div class="insight-copy">Your reporting rules still live in the application layer. This screen is focused on making those results easier to scan.</div>
      </article>
    </div>
  </div>`
})
export class ExpensesPage {}

@Component({
  standalone: true,
  imports: [CommonModule],
  template: `
  <div class="page">
    <div class="page-head">
      <div><h2 class="page-title">Investments</h2><p class="page-subtitle">A clean view of the holdings behind your portfolio.</p></div>
    </div>
    <div class="card list-card">
      <div class="section-card-head" style="padding:20px 20px 0"><div class="section-card-title">Holdings</div></div>
      <div class="data-list" *ngIf="holdings.length; else empty">
        <div class="data-row" *ngFor="let h of holdings">
          <div class="list-icon"><span class="icon-chart"></span></div>
          <div class="data-main">
            <div class="data-title">{{h.securityId}}</div>
            <div class="data-subtitle">{{h.quantity}} units</div>
          </div>
          <div class="data-value">{{money(h.costBasis)}}</div>
        </div>
      </div>
      <ng-template #empty>
        <div class="empty-card"><div class="empty-icon"><span class="icon-chart"></span></div><div class="empty-title">No holdings to show</div><div class="empty-copy">Once your investment ledger has data, your positions will appear here.</div></div>
      </ng-template>
    </div>
  </div>`
})
export class InvestmentsPage {
  private api = inject(ApiService);
  holdings: InvestmentHolding[] = [];
  money = money;

  constructor() { this.api.holdings().subscribe({ next: x => this.holdings = x, error: () => this.holdings = [] }); }
}

@Component({
  standalone: true,
  template: `
  <div class="page">
    <div class="page-head">
      <div><h2 class="page-title">Assets</h2><p class="page-subtitle">Bring your long-term assets into the same picture.</p></div>
    </div>
    <div class="card-grid two">
      <article class="card section-card">
        <div class="empty-card"><div class="empty-icon"><span class="icon-home"></span></div><div class="empty-title">Assets are ready to be surfaced</div><div class="empty-copy">Property and other tracked assets can be shown here with their latest valuations.</div></div>
      </article>
      <article class="card insight">
        <div class="insight-kicker">Long-term view</div>
        <div class="insight-title">Track the things that matter beyond your bank balance.</div>
        <div class="insight-copy">Acquisition values and historical valuations continue to come from the existing asset model.</div>
      </article>
    </div>
  </div>`
})
export class AssetsPage {}

@Component({
  standalone: true,
  template: `
  <div class="page">
    <div class="page-head">
      <div><h2 class="page-title">Liabilities</h2><p class="page-subtitle">Know what you owe and how it changes over time.</p></div>
    </div>
    <div class="card-grid two">
      <article class="card section-card">
        <div class="empty-card"><div class="empty-icon"><span class="icon-shield"></span></div><div class="empty-title">Liability tracking is in place</div><div class="empty-copy">Loans, repayments and outstanding balances can be brought into this view through the existing API.</div></div>
      </article>
      <article class="card insight">
        <div class="insight-kicker">Less noise</div>
        <div class="insight-title">A liability is easier to manage when the balance and repayment story are visible together.</div>
        <div class="insight-copy">The application continues to own the underlying calculations. The UI simply presents them clearly.</div>
      </article>
    </div>
  </div>`
})
export class LiabilitiesPage {}

@Component({
  standalone: true,
  imports: [CommonModule],
  template: `
  <div class="page">
    <div class="page-head">
      <div><h2 class="page-title">Imports</h2><p class="page-subtitle">Bring your financial data in without losing the validation guardrails.</p></div>
    </div>
    <article class="card form-card">
      <div class="section-card-head">
        <div>
          <div class="section-card-title">Import your workbook</div>
          <div class="page-subtitle">Use the supported Personal Wealth template. Validation stays non-destructive.</div>
        </div>
      </div>
      <input class="file-input" type="file" accept=".xlsx,.csv" (change)="select($event)">
      <div class="action-row">
        <button class="secondary-button" type="button" (click)="downloadTemplate()">Download template</button>
        <button class="primary-button" type="button" [disabled]="!file || busy" (click)="upload()">{{busy ? 'Working…' : 'Upload and import'}}</button>
      </div>
      <div class="status-note" *ngIf="message">{{message}}</div>
      <div class="status-note" *ngIf="result && !result.errors?.length">Imported {{result.rowsImported}} rows successfully.</div>
      <div *ngIf="result?.errors?.length">
        <div class="status-note">Please fix the reported issues and upload again.</div>
        <ol class="error-list"><li *ngFor="let error of result?.errors">{{error}}</li></ol>
      </div>
    </article>
  </div>`
})
export class ImportsPage {
  private api = inject(ApiService);
  file?: File;
  busy = false;
  message = '';
  result?: WealthImportResult;

  select(e: Event) {
    const i = e.target as HTMLInputElement;
    this.file = i.files?.[0];
    this.result = undefined;
    this.message = this.file ? this.file.name + ' selected' : '';
  }

  upload() {
    if (!this.file) return;
    this.busy = true;
    this.result = undefined;
    this.api.importWealth(this.file).subscribe({
      next: r => {
        this.result = r;
        this.busy = false;
        this.message = r.success ? 'Your financial data is ready.' : 'The import was blocked until the reported issues are corrected.';
      },
      error: e => {
        this.busy = false;
        this.message = e?.error?.errors?.join(' ') || 'Import failed. Your existing data was not changed.';
      }
    });
  }

  downloadTemplate() {
    const wb = XLSX.utils.book_new();
    const sheets: Record<string, unknown[][]> = {
      'Bank Accounts': [['ExternalId','Institution','AccountNumber','AccountType','Currency'],['bank-hdfc','HDFC Bank','HDFC-001','Savings','INR']],
      'Bank Transactions': [['ExternalId','Date','AccountNumber','Currency','Description','Direction','Amount','Category'],['txn-salary-2026-08-01','2026-08-01','HDFC-001','INR','Salary','Credit',150000,'Income'],['txn-rent-2026-08-05','2026-08-05','HDFC-001','INR','Rent','Debit',35000,'Housing']],
      'Expenses': [['ExternalId','Date','AccountNumber','Currency','Description','Amount','Category'],['exp-rent-2026-08-05','2026-08-05','HDFC-001','INR','Monthly rent',35000,'Housing']],
      'Investment Accounts': [['ExternalId','Institution','AccountNumber','AccountType','Currency'],['inv-zerodha','Zerodha','Z-001','Brokerage','INR']],
      'Securities': [['ExternalId','SecuritySymbol','SecurityName','SecurityType','Currency'],['sec-reliance','RELIANCE','Reliance Industries','Equity','INR']],
      'Investment Transactions': [['ExternalId','Date','AccountNumber','Currency','Description','Direction','SecuritySymbol','Quantity','UnitPrice','Fees'],['inv-buy-reliance','2026-08-10','Z-001','INR','Purchase','Buy','RELIANCE',20,2800,20]],
      'Assets': [['ExternalId','Date','AssetName','AssetType','Currency','AcquisitionValue'],['asset-home','2024-01-01','Home','Property','INR',8000000]],
      'Asset Valuations': [['ExternalId','Date','AssetName','ValuationValue'],['asset-home-val','2026-08-31','Home',8500000]],
      'Liabilities': [['ExternalId','Date','LiabilityName','LiabilityType','Currency','Principal','InterestRate','MaturityDate'],['loan-home','2024-01-01','Home Loan','Mortgage','INR',5000000,8.5,'2029-01-01']],
      'Liability Repayments': [['ExternalId','Date','LiabilityName','Currency','Amount','PrincipalAmount','InterestAmount'],['loan-home-repay-2026-08','2026-08-15','Home Loan','INR',50000,40000,10000]],
      'Instructions': [['Personal Wealth Import Template'],['Keep the financial sheet names and columns unchanged.'],['Enter one record per row and keep dates as YYYY-MM-DD.'],['Validation errors are reported with their sheet and Excel cell address.'],['No financial data is imported when workbook validation fails.']]
    };
    Object.entries(sheets).forEach(([name, data]) => {
      const ws = XLSX.utils.aoa_to_sheet(data);
      ws['!freeze'] = { xSplit: 0, ySplit: 1 };
      ws['!autofilter'] = { ref: XLSX.utils.encode_range({ s: { r: 0, c: 0 }, e: { r: 0, c: data[0].length - 1 } }) };
      XLSX.utils.book_append_sheet(wb, ws, name);
    });
    XLSX.writeFile(wb, 'PersonalWealth_Import_Template.xlsx');
  }
}