import React from 'react';
import { Link } from 'react-router-dom';

export const Footer = () => {
  return (
    <footer className="pf-footer">
      <div className="container">
        <div className="row g-4">

          {/* Col 1 — Logo + Description */}
          <div className="col-lg-4">
            <div className="d-flex align-items-center gap-2 mb-3">
              <div className="bg-primary text-white rounded d-flex align-items-center justify-content-center" style={{ width: '36px', height: '36px' }}>
                <i className="bi bi-gears" style={{ fontSize: '1.2rem' }}></i>
              </div>
              <span style={{ fontSize: '1.4rem', fontWeight: 800 }}>
                <span style={{ color: '#fff' }}>Part</span>
                <span style={{ color: 'var(--pf-gold)' }}>Finder</span>
                <span style={{ color: '#fff', fontSize: '1.2rem' }}>.ma</span>
              </span>
            </div>
            <p style={{ color: '#9CA3AF', fontSize: '0.88rem', lineHeight: 1.7 }}>
              Fournisseur de pièces industrielles et équipements électroniques au Maroc.
              Matériels neufs et reconditionnés — disponibilité vérifiée en temps réel.
            </p>
            <div className="d-flex gap-3 mt-3">
              {[
                { icon: 'bi-linkedin', href: 'https://www.linkedin.com/' },
                { icon: 'bi-facebook', href: 'https://www.facebook.com/' },
                { icon: 'bi-whatsapp', href: 'https://wa.me/212702380380' },
              ].map((s, i) => (
                <a key={i} href={s.href}
                  style={{
                    width: 36, height: 36, borderRadius: '50%',
                    background: 'rgba(255,255,255,0.08)',
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    color: '#9CA3AF', textDecoration: 'none', fontSize: '1rem',
                    transition: 'background 0.2s, color 0.2s'
                  }}
                  onMouseEnter={e => {
                    (e.currentTarget as HTMLElement).style.background = 'var(--pf-gold)';
                    (e.currentTarget as HTMLElement).style.color = '#fff';
                  }}
                  onMouseLeave={e => {
                    (e.currentTarget as HTMLElement).style.background = 'rgba(255,255,255,0.08)';
                    (e.currentTarget as HTMLElement).style.color = '#9CA3AF';
                  }}>
                  <i className={`bi ${s.icon}`}></i>
                </a>
              ))}
            </div>
          </div>

          {/* Col 2 — Navigation */}
          <div className="col-6 col-lg-2">
            <div className="pf-footer-title">Navigation</div>
            <nav className="d-flex flex-column">
              <Link to="/" className="pf-footer-link">Accueil</Link>
              <Link to="/catalogue" className="pf-footer-link">Catalogue</Link>
              <Link to="/cart" className="pf-footer-link">Mon panier</Link>
              <Link to="/login" className="pf-footer-link">Mon Espace</Link>
              <Link to="/register" className="pf-footer-link">Créer un compte</Link>
            </nav>
          </div>

          {/* Col 3 — Infos pratiques */}
          <div className="col-6 col-lg-3">
            <div className="pf-footer-title">Nos Services</div>
            <nav className="d-flex flex-column">
              <span className="pf-footer-link">Devis gratuit sous 24h</span>
              <span className="pf-footer-link">Matériels reconditionnés</span>
              <span className="pf-footer-link">Pièces de rechange</span>
              <span className="pf-footer-link">Sourcing sur mesure</span>
              <span className="pf-footer-link">Livraison Maroc entier</span>
            </nav>
          </div>

          {/* Col 4 — Contact */}
          <div className="col-lg-3">
            <div className="pf-footer-title">Contact</div>
            <div className="d-flex flex-column gap-2">
              {[
                { icon: 'bi-geo-alt-fill', text: 'Casablanca, Maroc' },
                { icon: 'bi-telephone-fill', text: '07 02 380 380' },
                { icon: 'bi-envelope-fill', text: 'info@partfinder.ma' },
                { icon: 'bi-clock-fill', text: 'Lun–Ven : 9h00–18h00 | Sam : 9h00–13h00' },
              ].map((c, i) => (
                <div key={i} className="d-flex align-items-start gap-2">
                  <i className={`bi ${c.icon} mt-1 flex-shrink-0`} style={{ color: 'var(--pf-gold)', fontSize: '0.85rem' }}></i>
                  <span style={{ color: '#9CA3AF', fontSize: '0.88rem' }}>{c.text}</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* Bottom bar */}
      <div className="pf-footer-bottom">
        <p className="mb-0">
          © {new Date().getFullYear()} PartFinder.ma — Tous droits réservés.
          &nbsp;|&nbsp; Propulsé par <strong style={{ color: 'var(--pf-gold)' }}>CapLed ERP</strong>
        </p>
      </div>
    </footer>
  );
};
