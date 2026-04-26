import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Navbar } from '../components/layout/Navbar';
import { Footer } from '../components/layout/Footer';
import { ProductCard } from '../components/catalogue/ProductCard';
import { catalogueApi } from '../api/catalogueApi';

const FAMILLE_ICONS: Record<string, string> = {
  default: 'bi-cpu',
  electric: 'bi-lightning-charge',
  motor: 'bi-gear-wide-connected',
  pump: 'bi-droplet',
  pneumatic: 'bi-wind',
  measure: 'bi-speedometer2',
  safety: 'bi-shield-check',
  network: 'bi-diagram-3',
};

const STEPS = [
  { icon: 'bi-search', num: '01', title: 'Recherchez', desc: 'Trouvez le bon équipement par référence, nom ou famille.' },
  { icon: 'bi-cart-check', num: '02', title: 'Ajoutez au panier', desc: 'Constituez votre liste de besoins en quelques clics.' },
  { icon: 'bi-file-earmark-text', num: '03', title: 'Demandez un devis', desc: 'Envoyez votre demande, nous vous répondons rapidement.' },
  { icon: 'bi-truck', num: '04', title: 'Recevez votre commande', desc: 'Livraison rapide partout au Maroc.' },
];

export const HomePage = () => {
  const navigate = useNavigate();
  const [familles, setFamilles] = useState<any[]>([]);
  const [latestProducts, setLatestProducts] = useState<any[]>([]);
  const [totalProducts, setTotalProducts] = useState(0);
  const [searchQuery, setSearchQuery] = useState('');

  useEffect(() => {
    catalogueApi.getFamilles().then(f => setFamilles(Array.isArray(f) ? f : [])).catch(() => {});
    catalogueApi.getCatalogue({ page: 1, pageSize: 8 })
      .then((r: any) => {
        setLatestProducts(r?.items?.slice(0, 8) || []);
        setTotalProducts(r?.totalCount || 0);
      })
      .catch(() => {});
  }, []);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim()) navigate(`/catalogue?search=${encodeURIComponent(searchQuery.trim())}`);
    else navigate('/catalogue');
  };

  return (
    <div className="d-flex flex-column min-vh-100">
      <Navbar />

      {/* ══════════════════════════════════
          HERO
      ══════════════════════════════════ */}
      <section className="pf-hero">
        <div className="container">
          <div className="row align-items-center g-5">
            {/* Left: Text + values */}
            <div className="col-lg-5">
              <span className="badge mb-3 px-3 py-2"
                style={{ background: '#EEF4FB', color: 'var(--pf-navy)', fontSize: '0.82rem', fontWeight: 600, borderRadius: '20px' }}>
                <i className="bi bi-award me-1"></i>Équipements industriels certifiés
              </span>
              <h1 className="pf-hero-title mb-4">
                Pièces industrielles &amp;<br />équipements au <span style={{ color: 'var(--pf-gold)' }}>Maroc</span>
              </h1>
              <ul className="pf-hero-values mb-5">
                {[
                  'Livraison rapide sur tout le Maroc',
                  'Matériels neufs et reconditionnés',
                  'Devis gratuit sous 24h',
                  'Support technique spécialisé',
                ].map((v, i) => (
                  <li key={i} className="pf-hero-value-item">
                    <span className="pf-hero-value-icon"><i className="bi bi-check-lg"></i></span>
                    {v}
                  </li>
                ))}
              </ul>
              <div className="d-flex flex-wrap gap-2" style={{ marginTop: '8px' }}>
                <Link to="/catalogue" className="btn btn-primary px-4 fw-bold" style={{ whiteSpace: 'nowrap' }}>
                  <i className="bi bi-grid-3x3-gap me-2"></i>Voir le catalogue
                </Link>
                <Link to="/contact" className="btn btn-outline-secondary px-4 fw-bold" style={{ whiteSpace: 'nowrap' }}>
                  <i className="bi bi-telephone me-2"></i>Nous contacter
                </Link>
              </div>
            </div>

            {/* Right: Search box + featured image */}
            <div className="col-lg-7">
              <div className="bg-white rounded-3 shadow p-4">
                <p className="fw-bold text-dark mb-3" style={{ fontSize: '0.95rem' }}>
                  <i className="bi bi-search me-2" style={{ color: 'var(--pf-navy)' }}></i>
                  Recherche rapide par référence ou désignation
                </p>
                <form onSubmit={handleSearch}>
                  <div className="pf-hero-search mb-3">
                    <input
                      type="text"
                      className="pf-hero-search-input"
                      placeholder="Ex : Variateur, Automate, Contacteur..."
                      value={searchQuery}
                      onChange={e => setSearchQuery(e.target.value)}
                    />
                    <button type="submit" className="pf-hero-search-btn">
                      <i className="bi bi-search me-2"></i>Rechercher
                    </button>
                  </div>
                </form>
                {/* Quick family links */}
                <div className="d-flex flex-wrap gap-2">
                  {familles.slice(0, 5).map(f => (
                    <Link
                      key={f.id}
                      to={`/catalogue?familleId=${f.id}`}
                      className="badge text-decoration-none py-2 px-3"
                      style={{ background: '#F1F5F9', color: 'var(--pf-navy)', fontWeight: 600, fontSize: '0.78rem', borderRadius: '20px' }}>
                      <i className="bi bi-tools me-1"></i>{f.libelle || f.code}
                    </Link>
                  ))}
                  {familles.length > 5 && (
                    <Link to="/catalogue" className="badge text-decoration-none py-2 px-3"
                      style={{ background: 'var(--pf-navy)', color: '#fff', fontWeight: 600, fontSize: '0.78rem', borderRadius: '20px' }}>
                      +{familles.length - 5} autres
                    </Link>
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* ══════════════════════════════════
          STATS BAR
      ══════════════════════════════════ */}
      <div className="pf-stats-bar">
        <div className="container">
          <div className="row g-0">
            {[
              { value: totalProducts > 0 ? `${totalProducts}+` : '—', label: 'Références disponibles' },
              { value: familles.length > 0 ? `${familles.length}` : '—', label: 'Familles de produits' },
              { value: '24h', label: 'Délai de réponse devis' },
              { value: '100%', label: 'Pièces testées et vérifiées' },
            ].map((s, i) => (
              <div key={i} className="col-6 col-md-3 pf-stat-item py-2">
                <span className="pf-stat-value">{s.value}</span>
                <span className="pf-stat-label">{s.label}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* ══════════════════════════════════
          FAMILLES GRID
      ══════════════════════════════════ */}
      {familles.length > 0 && (
        <section className="py-5" style={{ background: '#fff' }}>
          <div className="container">
            <div className="text-center mb-2">
              <h2 className="pf-section-title">Nos Familles de Produits</h2>
              <p className="pf-section-subtitle">Parcourez notre catalogue par catégorie d'équipements industriels</p>
            </div>
            <div className="row row-cols-2 row-cols-md-3 row-cols-lg-4 g-3">
              {familles.map((f, i) => (
                <div key={f.id} className="col">
                  <Link to={`/catalogue?familleId=${f.id}`} className="pf-famille-card">
                    <div className="pf-famille-icon">
                      <i className={`bi ${Object.values(FAMILLE_ICONS)[i % Object.values(FAMILLE_ICONS).length]}`}></i>
                    </div>
                    <span className="pf-famille-label">{f.libelle || f.code}</span>
                  </Link>
                </div>
              ))}
              {/* "All products" card */}
              <div className="col">
                <Link to="/catalogue" className="pf-famille-card" style={{ borderStyle: 'dashed' }}>
                  <div className="pf-famille-icon" style={{ background: '#FFF8EC', color: 'var(--pf-gold)' }}>
                    <i className="bi bi-grid-3x3-gap"></i>
                  </div>
                  <span className="pf-famille-label">Tout voir</span>
                </Link>
              </div>
            </div>
          </div>
        </section>
      )}

      {/* ══════════════════════════════════
          DERNIERS PRODUITS
      ══════════════════════════════════ */}
      {latestProducts.length > 0 && (
        <section className="py-5" style={{ background: 'var(--pf-gray-bg)' }}>
          <div className="container">
            <div className="d-flex justify-content-between align-items-end mb-4">
              <div>
                <h2 className="pf-section-title mb-1">Derniers équipements ajoutés</h2>
                <p className="pf-section-subtitle mb-0">Découvrez nos dernières références disponibles</p>
              </div>
              <Link to="/catalogue" className="btn btn-outline-primary fw-bold d-none d-md-inline-flex align-items-center gap-2">
                Voir tout <i className="bi bi-arrow-right"></i>
              </Link>
            </div>
            <div className="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-xl-4 g-3">
              {latestProducts.map(p => (
                <div key={p.id} className="col">
                  <ProductCard product={p} />
                </div>
              ))}
            </div>
            <div className="text-center mt-4 d-md-none">
              <Link to="/catalogue" className="btn btn-primary fw-bold px-5">Voir tout le catalogue</Link>
            </div>
          </div>
        </section>
      )}

      {/* ══════════════════════════════════
          HOW IT WORKS
      ══════════════════════════════════ */}
      <section className="py-5" style={{ background: '#fff', borderTop: '1px solid var(--pf-border)' }}>
        <div className="container">
          <div className="text-center mb-5">
            <h2 className="pf-section-title">Comment ça marche ?</h2>
            <p className="pf-section-subtitle">Un processus simple et transparent pour votre approvisionnement</p>
          </div>
          <div className="row g-4">
            {STEPS.map((step, i) => (
              <div key={i} className="col-6 col-md-3 text-center">
                <div className="pf-step-circle">
                  <i className={`bi ${step.icon}`}></i>
                </div>
                <div className="pf-step-title">{step.num} — {step.title}</div>
                <p className="pf-step-desc">{step.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ══════════════════════════════════
          CTA BAND
      ══════════════════════════════════ */}
      <section style={{ background: 'var(--pf-navy)', padding: '48px 0' }}>
        <div className="container text-center">
          <h2 className="text-white fw-bold mb-3" style={{ fontSize: 'var(--text-2xl)' }}>
            Vous ne trouvez pas ce que vous cherchez ?
          </h2>
          <p className="mb-4" style={{ color: 'rgba(255,255,255,0.6)', fontSize: 'var(--text-base)' }}>
            Notre équipe identifie et sourcings la pièce pour vous, même hors catalogue.
          </p>
          <Link to="/contact" className="btn btn-warning fw-bold px-5" style={{ borderRadius: 'var(--radius-sm)' }}>
            <i className="bi bi-telephone-fill me-2"></i>Contacter notre équipe
          </Link>
        </div>
      </section>

      <Footer />
    </div>
  );
};
