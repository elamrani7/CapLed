import React, { useEffect, useMemo, useState } from 'react';
import { resolveAssetUrl } from '../../api/assets';

type ProductImageGalleryProps = {
  images?: string[];
  reference: string;
};

export const ProductImageGallery = ({ images, reference }: ProductImageGalleryProps) => {
  const [activeIndex, setActiveIndex] = useState(0);
  const [failedImages, setFailedImages] = useState<Set<string>>(new Set());

  const resolvedImages = useMemo(
    () => (images || []).map(resolveAssetUrl).filter((url): url is string => !!url),
    [images]
  );
  const visibleImages = resolvedImages.filter((url) => !failedImages.has(url));

  useEffect(() => {
    if (activeIndex >= visibleImages.length) {
      setActiveIndex(0);
    }
  }, [activeIndex, visibleImages.length]);

  const markImageFailed = (url: string) => {
    setFailedImages((current) => new Set(current).add(url));
  };

  if (visibleImages.length === 0) {
    return (
      <div className="pf-gallery-empty">
        <div className="pf-gallery-empty-content">
          <i className="bi bi-image"></i>
          <span>Image non disponible</span>
          <small>{reference}</small>
        </div>
      </div>
    );
  }

  const mainImage = visibleImages[activeIndex] || visibleImages[0];

  return (
    <div className="pf-gallery">
      {visibleImages.length > 1 && (
        <div className="pf-gallery-thumbs" aria-label="Autres vues du produit">
          {visibleImages.map((img, idx) => (
            <button
              key={img}
              type="button"
              onClick={() => setActiveIndex(idx)}
              className={`pf-gallery-thumb ${activeIndex === idx ? 'active' : ''}`}
              aria-label={`Afficher la vue ${idx + 1}`}
            >
              <img
                src={img}
                alt={`${reference} vue ${idx + 1}`}
                className="w-100 h-100 object-fit-cover"
                onError={() => markImageFailed(img)}
              />
            </button>
          ))}
        </div>
      )}

      <div className="pf-gallery-main">
        <img
          src={mainImage}
          alt={`Aperçu ${reference}`}
          onError={() => markImageFailed(mainImage)}
        />
      </div>
    </div>
  );
};
