import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useCart } from '../../context/CartContext';
import { catalogueApi } from '../../api/catalogueApi';

export const Navbar = () => {
  const { totalItems } = useCart();
  const navigate = useNavigate();
  
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedSearchFamille, setSelectedSearchFamille] = useState('');
  
  // Header data
  const [familles, setFamilles] = useState<any[]>([]);
  const [categories, setCategories] = useState<any[]>([]);
  const [hoveredFamille, setHoveredFamille] = useState<number | null>(null);

  useEffect(() => {
    let isMounted = true;
    const fetchNavData = async () => {
      try {
        const fams = await catalogueApi.getFamilles();
        const cats = await catalogueApi.getCategories();
        if (isMounted) {
          setFamilles(Array.isArray(fams) ? fams : []);
          setCategories(Array.isArray(cats) ? cats : []);
        }
      } catch (err) {
        console.warn('Could not load menu items', err);
      }
    };
    fetchNavData();
    return () => { isMounted = false; };
  }, []);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchQuery.trim() || selectedSearchFamille) {
      const params = new URLSearchParams();
      if (searchQuery.trim()) params.append('search', searchQuery.trim());
      if (selectedSearchFamille) params.append('familleId', selectedSearchFamille);
      
      navigate(`/catalogue?${params.toString()}`);
      setSearchQuery('');
    }
  };

  return (
    <header>
      {/* TIER 1 - Main Search Header */}
      <div className="autodoc-tier1">
        <div className="container-fluid px-4">
          <div className="row align-items-center">
            
            {/* Logo Left */}
            <div className="col-12 col-lg-2 mb-3 mb-lg-0 d-flex justify-content-center justify-content-lg-start align-items-center">
              <Link to="/" className="text-white text-decoration-none d-flex align-items-center gap-2 fw-bolder fs-3">
                <i className="bi bi-gear-wide-connected" style={{ color: '#ff7a00' }}></i>
                CapLed
              </Link>
            </div>

            {/* Center Massive Search */}
            <div className="col-12 col-lg-7 mb-3 mb-lg-0">
              <form onSubmit={handleSearchSubmit}>
                <div className="autodoc-search-container shadow-sm">
                  {/* Left Dropdown attached to search bar */}
                  <select 
                    className="autodoc-search-select d-none d-md-block"
                    value={selectedSearchFamille}
                    onChange={(e) => setSelectedSearchFamille(e.target.value)}
                  >
                    <option value="">Vos équipements</option>
                    {familles.map(f => (
                      <option key={f.id} value={f.id}>{f.libelle || f.code}</option>
                    ))}
                  </select>
                  
                  {/* Free text input */}
                  <input 
                    type="text" 
                    className="autodoc-search-input" 
                    placeholder="Entrez le numéro ou le nom de la pièce"
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                  />
                  
                  {/* Big Search Action */}
                  <button type="submit" className="autodoc-search-btn">
                    <i className="bi bi-search d-block d-sm-none"></i>
                    <span className="d-none d-sm-block"><i className="bi bi-search me-2"></i> RECHERCHER</span>
                  </button>
                </div>
              </form>
            </div>

            {/* Right Actions */}
            <div className="col-12 col-lg-3 d-flex justify-content-center justify-content-lg-end align-items-center gap-4">
              
              <Link to="/cart" className="text-white text-decoration-none d-flex align-items-center gap-3">
                <div className="position-relative">
                  <i className="bi bi-cart3 fs-3"></i>
                  <span className="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-warning text-dark border border-dark">
                    {totalItems}
                  </span>
                </div>
                <div className="d-none d-xl-block lh-sm text-start">
                  <small className="text-secondary d-block" style={{ fontSize: '0.75rem' }}>article(s)</small>
                  <span className="fw-bolder">Devis</span>
                </div>
              </Link>

              <div className="text-white d-flex align-items-center gap-3" style={{ cursor: 'pointer' }}>
                <i className="bi bi-person-circle fs-3 text-secondary"></i>
                <div className="d-none d-xl-block lh-sm text-start">
                  <span className="fw-bolder d-block text-light" style={{ fontSize: '0.85rem' }}>Mon Espace</span>
                  <small className="text-secondary" style={{ fontSize: '0.75rem' }}>Se connecter</small>
                </div>
              </div>

            </div>
          </div>
        </div>
      </div>

      {/* TIER 2 - Horizontal Family Navigation */}
      <div className="autodoc-tier2">
        <div className="container-fluid px-4">
          <ul className="autodoc-nav-list">
            <li className="autodoc-nav-item border-end border-secondary border-opacity-25">
              <Link to="/catalogue" className="autodoc-nav-link text-white fw-bold">
                <i className="bi bi-ui-radios-grid me-1"></i> Tous les équipements
              </Link>
            </li>
            {familles.map(f => (
              <li 
                className="autodoc-nav-item dropdown dropdown-hover" 
                key={f.id}
                onMouseEnter={() => setHoveredFamille(f.id)}
                onMouseLeave={() => setHoveredFamille(null)}
              >
                <Link 
                  to={`/catalogue?familleId=${f.id}`} 
                  className="autodoc-nav-link"
                >
                  <i className="bi bi-tools opacity-50"></i> {f.libelle || f.code}
                </Link>
                {hoveredFamille === f.id && (
                  <div className="dropdown-menu show shadow mt-0 border-0 rounded-0" style={{ position: 'absolute', minWidth: '300px', zIndex: 1050 }}>
                    {categories.filter(c => c.familleId === f.id).length > 0 ? (
                      categories.filter(c => c.familleId === f.id).map(cat => (
                        <Link 
                          key={cat.id} 
                          to={`/catalogue?familleId=${f.id}&categorieId=${cat.id}`} 
                          className="dropdown-item py-2 fw-medium"
                        >
                          {cat.label}
                        </Link>
                      ))
                    ) : (
                      <span className="dropdown-item text-muted disabled">Découvrir cette famille</span>
                    )}
                  </div>
                )}
              </li>
            ))}
          </ul>
        </div>
      </div>
    </header>
  );
};
