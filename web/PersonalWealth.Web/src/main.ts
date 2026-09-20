import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Component, HostListener } from '@angular/core';
import { routes } from './app/routes';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="app-shell" [class.dark-theme]="darkMode">
      <aside class="sidebar">
        <div class="brand">
          <div class="brand-mark" aria-hidden="true">
            <span></span><span></span><span></span>
          </div>
          <div>
            <div class="brand-name">Personal Wealth</div>
            <div class="brand-subtitle">Your money, in one place</div>
          </div>
        </div>

        <div class="nav-label">Overview</div>
        <nav class="primary-nav" aria-label="Primary navigation">
          <a routerLink="/" routerLinkActive="active" [routerLinkActiveOptions]="{exact:true}">
            <span class="nav-icon icon-grid" aria-hidden="true"></span><span>Dashboard</span>
          </a>
          <a routerLink="/banking" routerLinkActive="active">
            <span class="nav-icon icon-bank" aria-hidden="true"></span><span>Banking</span>
          </a>
          <a routerLink="/expenses" routerLinkActive="active">
            <span class="nav-icon icon-wallet" aria-hidden="true"></span><span>Expenses</span>
          </a>
          <a routerLink="/investments" routerLinkActive="active">
            <span class="nav-icon icon-chart" aria-hidden="true"></span><span>Investments</span>
          </a>
          <a routerLink="/assets" routerLinkActive="active">
            <span class="nav-icon icon-home" aria-hidden="true"></span><span>Assets</span>
          </a>
          <a routerLink="/liabilities" routerLinkActive="active">
            <span class="nav-icon icon-shield" aria-hidden="true"></span><span>Liabilities</span>
          </a>
        </nav>

        <div class="nav-label">Tools</div>
        <nav class="primary-nav">
          <a routerLink="/imports" routerLinkActive="active">
            <span class="nav-icon icon-upload" aria-hidden="true"></span><span>Imports</span>
          </a>
        </nav>

        <div class="sidebar-card">
          <div class="sidebar-card-icon"><span class="icon-spark"></span></div>
          <div class="sidebar-card-title">Build your wealth story</div>
          <div class="sidebar-card-text">Keep your accounts, investments and goals visible at a glance.</div>
        </div>

        <div class="sidebar-footer">
          <div class="profile">
            <div class="profile-avatar">S</div>
            <div>
              <div class="profile-name">Swapnil</div>
              <div class="profile-role">Personal workspace</div>
            </div>
          </div>
          <button class="theme-switch" type="button" (click)="toggleTheme()" [attr.aria-label]="darkMode ? 'Switch to light mode' : 'Switch to dark mode'">
            <span class="icon-sun"></span>
            <span class="switch-track" [class.on]="darkMode"><span class="switch-thumb"></span></span>
            <span class="icon-moon"></span>
          </button>
        </div>
      </aside>

      <main class="content">
        <header class="topbar">
          <div>
            <div class="eyebrow">Sunday, September 20</div>
            <h1>Good afternoon, Swapnil</h1>
          </div>
          <div class="topbar-actions">
            <button class="icon-button" type="button" aria-label="Notifications"><span class="icon-bell"></span></button>
            <button class="avatar-button" type="button" aria-label="Profile">S</button>
          </div>
        </header>

        <section class="content-wrap">
          <router-outlet />
        </section>
      </main>
    </div>
  `
})
class AppComponent {
  darkMode = localStorage.getItem('pw-theme') === 'dark';

  toggleTheme() {
    this.darkMode = !this.darkMode;
    localStorage.setItem('pw-theme', this.darkMode ? 'dark' : 'light');
  }

  @HostListener('window:storage', ['$event'])
  onStorage(event: StorageEvent) {
    if (event.key === 'pw-theme') {
      this.darkMode = event.newValue === 'dark';
    }
  }
}

bootstrapApplication(AppComponent, {
  providers: [provideHttpClient(), provideRouter(routes)]
});